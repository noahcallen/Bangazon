using Bangazon.Modules;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);


// Enable OpenAPI (Swagger)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Allow passing DateTimes without timezone data
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Connect API to PostgreSQL Database
builder.Services.AddNpgsql<BangazonDbContext>(builder.Configuration["BangazonDbConnectionString"]);

// Set JSON serialization options (Prevents circular JSON errors)
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// ✅ CORS Policy (Allow frontend at `localhost:3000`)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000") // ✅ Adjusted origin for your frontend
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

var app = builder.Build();

// ✅ Enable CORS Middleware BEFORE routing
app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ USER Calls

// Check User
app.MapGet("/checkuser/{userId}", async (BangazonDbContext db, string userId) =>
{
    var exists = await db.Users.AnyAsync(u => u.Uid == userId);
    return exists ? Results.Ok() : Results.NotFound();
});

// Register User
app.MapPost("/users", async (BangazonDbContext db, User user) =>
{
    var existingUser = await db.Users.FindAsync(user.Uid);
    if (existingUser != null) return Results.BadRequest("User already exists.");

    try
    {
        db.Users.Add(user);
        await db.SaveChangesAsync();
        return Results.Created($"/users/{user.Uid}", user);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error registering user: {ex.Message}");
    }
});

// Get User Details
app.MapGet("/users/userdetails/{uid}", async (BangazonDbContext db, string uid) =>
{
    var user = await db.Users.FirstOrDefaultAsync(u => u.Uid == uid);
    return user != null ? Results.Ok(user) : Results.NotFound();
});

// ✅ CART Calls

// Get Customer Cart
app.MapGet("/cart/{userId}", async (BangazonDbContext db, string userId) =>
{
    var cart = await db.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    return cart != null ? Results.Ok(cart) : Results.NotFound();
});

// Add to Cart
app.MapPost("/cart/add", async (BangazonDbContext db, string userId, int productId, int quantity) =>
{
    var cart = await db.Carts.FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        db.Carts.Add(cart);
        await db.SaveChangesAsync();
    }

    var cartItem = await db.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);

    if (cartItem == null)
    {
        cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };
        db.CartItems.Add(cartItem);
    }
    else
    {
        cartItem.Quantity += quantity;
    }

    await db.SaveChangesAsync();
    return Results.Ok(cart);
});

// ✅ ORDER Calls

// Get Orders by Seller
app.MapGet("/orders/sellers/{sellerId}", async (BangazonDbContext db, string sellerId) =>
{
    var orders = await db.Orders
        .Where(o => o.IsComplete && o.OrderItems.Any(oi => oi.SellerId == sellerId))
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ToListAsync();

    return orders.Any() ? Results.Ok(orders) : Results.NotFound();
});

// Complete Order (Moves Items to Order and Clears the Cart)
app.MapPost("/orders/complete", async (BangazonDbContext db, string userId) =>
{
    var cart = await db.Carts.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.UserId == userId);
    if (cart == null || !cart.CartItems.Any()) return Results.BadRequest("Cart is empty");

    var order = new Order
    {
        CustomerId = userId,
        UserPaymentMethodId = cart.UserPaymentMethodId,
        IsComplete = false,
        OrderDate = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    var orderItems = cart.CartItems.Select(ci => new OrderItem
    {
        OrderId = order.Id,
        ProductId = ci.ProductId,
        Quantity = ci.Quantity,
        SellerId = db.Products.FirstOrDefault(p => p.Id == ci.ProductId)?.SellerId ?? ""
    }).ToList();

    db.OrderItems.AddRange(orderItems);
    db.CartItems.RemoveRange(cart.CartItems);
    db.Carts.Remove(cart);
    await db.SaveChangesAsync();

    return Results.Ok(order);
});

// ✅ PRODUCT Calls

// Get All Products
app.MapGet("/products", async (BangazonDbContext db) =>
{
    return Results.Ok(await db.Products.ToListAsync());
});

// Get Product by Id
app.MapGet("/products/{id}", async (BangazonDbContext db, int id) =>
{
    var product = await db.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
    return product != null ? Results.Ok(product) : Results.NotFound();
});

// ✅ SEARCH Calls

// Search Products
app.MapGet("/products/search", async (BangazonDbContext db, string? searchTerm) =>
{
    var products = string.IsNullOrWhiteSpace(searchTerm)
        ? await db.Products.Include(p => p.Category).ToListAsync()
        : await db.Products.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
                           .Include(p => p.Category)
                           .ToListAsync();

    return products.Any() ? Results.Ok(products) : Results.NotFound("No products found.");
});

// Search Sellers
app.MapGet("/sellers/search", async (BangazonDbContext db, string searchTerm) =>
{
    var sellerUids = await db.Users
        .Where(u => u.FirstName.ToLower().Contains(searchTerm.ToLower()) || u.LastName.ToLower().Contains(searchTerm.ToLower()))
        .Select(u => u.Uid)
        .ToListAsync();

    if (!sellerUids.Any()) return Results.NotFound("No sellers found.");

    var sellers = await db.Users.Where(u => sellerUids.Contains(u.Uid))
        .Select(u => new { u.Uid, u.FirstName, u.LastName, u.Email })
        .ToListAsync();

    return sellers.Any() ? Results.Ok(sellers) : Results.NotFound("No matching sellers with products found.");
});

app.Run();
