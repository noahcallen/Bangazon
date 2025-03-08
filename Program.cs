using Bangazon.Modules;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore.Query;
using Npgsql.Internal;
using System.Security.Cryptography;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<BangazonDbContext>(builder.Configuration["BangazonDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// Configure CORS policy to allow frontend requests
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.WithOrigins("http://localhost:3001")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Apply CORS before routing
app.UseCors();

app.UseHttpsRedirection();

// CART Calls

// ✅ GET Customer Cart
app.MapGet("/api/cart/{userId}", (BangazonDbContext db, string userId) =>
{
    var cart = db.Carts
        .Include(c => c.CartItems)
        .ThenInclude(ci => ci.Product)
        .FirstOrDefault(c => c.UserId == userId);

    return cart != null ? Results.Ok(cart) : Results.NotFound();
});

// ✅ Add to Cart
app.MapPost("/api/cart/add", (BangazonDbContext db, string userId, int productId, int quantity) =>
{
    var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);

    if (cart == null)
    {
        cart = new Cart { UserId = userId };
        db.Carts.Add(cart);
        db.SaveChanges(); // ✅ Ensure cart.Id is saved
    }

    var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == productId);

    if (cartItem == null)
    {
        cartItem = new CartItem { CartId = cart.Id, ProductId = productId, Quantity = quantity };
        db.CartItems.Add(cartItem);
    }
    else
    {
        cartItem.Quantity += quantity;
    }

    db.SaveChanges();
    return Results.Ok(cart);
});

// ✅ Add Payment Method to Cart
app.MapPost("/api/cart/add-payment", (BangazonDbContext db, string userId, int paymentMethodId) =>
{
    var cart = db.Carts.FirstOrDefault(c => c.UserId == userId);
    if (cart == null) return Results.NotFound();
    
    cart.UserPaymentMethodId = paymentMethodId;
    db.SaveChanges();

    return Results.Ok(cart);
});

// ✅ DELETE Item from Cart
app.MapDelete("/api/cart/delete-item", (BangazonDbContext db, int cartItemId) =>
{
    var cartItem = db.CartItems.SingleOrDefault(ci => ci.Id == cartItemId);
    if (cartItem == null) return Results.NotFound("Cart item not found.");

    db.CartItems.Remove(cartItem);
    db.SaveChanges();

    return Results.NoContent();
});

// ORDER Calls

// ✅ GET Orders by Seller
app.MapGet("/api/orders/sellers/{sellerId}", (BangazonDbContext db, string sellerId) =>
{
    var orders = db.Orders
        .Where(o => o.IsComplete && o.OrderItems.Any(oi => oi.SellerId == sellerId))
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ToList();

    return orders.Any() ? Results.Ok(orders) : Results.NotFound();
});

// ✅ POST then Complete Order (Moves Items to Order and Clears the Cart)
app.MapPost("/api/orders/complete", (BangazonDbContext db, string userId) =>
{
    var cart = db.Carts.Include(c => c.CartItems).FirstOrDefault(c => c.UserId == userId);
    if (cart == null || !cart.CartItems.Any()) return Results.BadRequest("Cart is empty");

    var order = new Order
    {
        CustomerId = userId,
        UserPaymentMethodId = cart.UserPaymentMethodId,
        IsComplete = false,
        OrderDate = DateTime.UtcNow
    };

    db.Orders.Add(order);
    db.SaveChanges(); // ✅ Ensure order.Id is saved

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
    db.SaveChanges();

    return Results.Ok(order);
});

// ✅ Confirm Order Payment
app.MapPost("/api/orders/confirm-payment/{orderId}", (BangazonDbContext db, int orderId) =>
{
    var order = db.Orders.FirstOrDefault(o => o.Id == orderId);
    if (order == null) return Results.NotFound("Order not found.");

    order.IsComplete = true;
    db.SaveChanges();

    return Results.Ok(order);
});

// ✅ GET Orders by Customer
app.MapGet("/api/orders/{id}", (BangazonDbContext db, string id) =>
{
    var orders = db.Orders
        .Where(o => o.CustomerId == id)
        .Include(o => o.OrderItems)
        .ThenInclude(oi => oi.Product)
        .ToList();

    return orders.Any() ? Results.Ok(orders) : Results.NotFound();
});

// PRODUCT Calls

// ✅ GET All Products
app.MapGet("/api/products", (BangazonDbContext db) =>
{
    return Results.Ok(db.Products.ToList());
});

// ✅ GET Products by Id
app.MapGet("/api/products/{id}", (BangazonDbContext db, int id) =>
{
    var product = db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
    return product != null ? Results.Ok(product) : Results.NotFound();
});

// ✅ GET Products by Category
app.MapGet("/api/products/category/{categoryId}", (BangazonDbContext db, int categoryId) =>
{
    var products = db.Products.Where(p => p.CategoryId == categoryId).Include(p => p.Category).ToList();
    return products.Any() ? Results.Ok(products) : Results.NotFound($"No products found in category {categoryId}.");
});

// ✅ GET 20 Latest Products
app.MapGet("/api/products/latest", (BangazonDbContext db) =>
{
    var latestProducts = db.Products.OrderByDescending(p => p.Id).Take(20).Include(p => p.Category).ToList();
    return Results.Ok(latestProducts);
});

// USER Calls

// ✅ Check User
app.MapGet("/api/checkuser/{userId}", (BangazonDbContext db, string userId) =>
{
    return db.Users.Any(u => u.Uid == userId) ? Results.Ok() : Results.NotFound();
});

// ✅ Register User
app.MapPost("/api/register", (BangazonDbContext db, User user) =>
{
    if (db.Users.Any(u => u.Uid == user.Uid))
    {
        return Results.BadRequest("User already exists.");
    }

    db.Users.Add(user);
    db.SaveChanges();
    return Results.Created($"/users/{user.Uid}", user);
});

// ✅ GET User Details
app.MapGet("/api/users/userdetails/{uid}", (BangazonDbContext db, string uid) =>
{
    var user = db.Users.FirstOrDefault(u => u.Uid == uid);
    return user != null ? Results.Ok(user) : Results.NotFound();
});

// ✅ Search Products
app.MapGet("/api/products/search", (BangazonDbContext db, string? searchTerm) =>
{
    if (string.IsNullOrWhiteSpace(searchTerm))
    {
        return Results.Ok(db.Products.Include(p => p.Category).ToList());
    }

    var products = db.Products
        .Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()))
        .Include(p => p.Category)
        .ToList();

    return products.Any() ? Results.Ok(products) : Results.NotFound("No products found.");
});

// ✅ Search Sellers
app.MapGet("/api/sellers/search", (BangazonDbContext db, string searchTerm) =>
{
    var sellerUids = db.Users
        .Where(u => u.FirstName.ToLower().Contains(searchTerm.ToLower()) || u.LastName.ToLower().Contains(searchTerm.ToLower()))
        .Select(u => u.Uid)
        .ToList();

    if (!sellerUids.Any()) return Results.NotFound("No sellers found.");

    var sellers = db.Users.Where(u => sellerUids.Contains(u.Uid)).Select(u => new
    {
        u.Uid, u.FirstName, u.LastName, u.Email
    }).ToList();

    return sellers.Any() ? Results.Ok(sellers) : Results.NotFound("No matching sellers with products found.");
});



app.Run();