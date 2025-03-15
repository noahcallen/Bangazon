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

//  CORS Policy (Allow frontend at `localhost:3000`)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy.WithOrigins("http://localhost:3000") //  Adjusted origin for your frontend
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
app.MapPost("/cart/add", async (BangazonDbContext db, HttpContext context) =>
{
    var requestData = await context.Request.ReadFromJsonAsync<CartRequest>();
    if (requestData == null) return Results.BadRequest("Invalid request format.");

    var cart = db.Carts.FirstOrDefault(c => c.UserId == requestData.UserId);

    if (cart == null)
    {
        cart = new Cart { UserId = requestData.UserId };
        db.Carts.Add(cart);
        db.SaveChanges();
    }

    var cartItem = db.CartItems.FirstOrDefault(ci => ci.CartId == cart.Id && ci.ProductId == requestData.ProductId);

    if (cartItem == null)
    {
        cartItem = new CartItem { CartId = cart.Id, ProductId = requestData.ProductId, Quantity = requestData.Quantity };
        db.CartItems.Add(cartItem);
    }
    else
    {
        cartItem.Quantity += requestData.Quantity;
    }

    db.SaveChanges();
    return Results.Ok(cart);
});

// ✅ Define a DTO (Data Transfer Object) for the Request
// Move this class outside the top-level statements


app.MapPut("/cart/update/{userId}", async (BangazonDbContext db, string userId, HttpContext context) =>
{
    var requestData = await context.Request.ReadFromJsonAsync<UpdateCartRequest>();
    if (requestData == null) return Results.BadRequest("Invalid request format.");

    // ✅ Ensure the user's cart exists
    var cart = await db.Carts
        .Include(c => c.CartItems)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null) return Results.NotFound($"No cart found for user {userId}.");

    // ✅ Find the cart item by ProductId
    var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == requestData.ProductId);
    if (cartItem == null) return Results.NotFound($"Product with ID {requestData.ProductId} not found in cart.");

    // ✅ If quantity is 0, remove item from cart
    if (requestData.NewQuantity <= 0)
    {
        db.CartItems.Remove(cartItem);
    }
    else
    {
        cartItem.Quantity = requestData.NewQuantity;
    }

    await db.SaveChangesAsync();
    return Results.Ok(cart);
});




// ✅ Remove a Single Product from the Cart
app.MapDelete("/cart/delete/{userId}/{productId}", async (BangazonDbContext db, string userId, int productId) =>
{
    var cart = await db.Carts
        .Include(c => c.CartItems)
        .FirstOrDefaultAsync(c => c.UserId == userId);

    if (cart == null) return Results.NotFound("Cart not found.");

    // ✅ Find the cart item by ProductId
    var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId);
    if (cartItem == null) return Results.NotFound($"Product with ID {productId} not found in cart.");

    // ✅ Remove the single product from the cart
    db.CartItems.Remove(cartItem);
    await db.SaveChangesAsync();

    return Results.NoContent();
});



// ✅ ORDER Calls



app.MapPost("/orders/complete", async (BangazonDbContext db, HttpContext context) =>
{
    var requestData = await context.Request.ReadFromJsonAsync<CompleteOrderRequest>();
    if (requestData == null) return Results.BadRequest("Invalid request format.");

    var cart = await db.Carts
        .Include(c => c.CartItems)
        .FirstOrDefaultAsync(c => c.UserId == requestData.UserId);

    if (cart == null || !cart.CartItems.Any()) 
        return Results.BadRequest("Cart is empty or does not exist.");

    // 🔥 Instead of using "paymentMethodId", we fetch "paymentOptionId" from UserPaymentMethods
    var userPaymentMethod = await db.UserPaymentMethods
        .FirstOrDefaultAsync(upm => upm.UserId == requestData.UserId && upm.PaymentOptionId == requestData.PaymentOptionId);

    if (userPaymentMethod == null)
        return Results.BadRequest("Invalid payment option. The user must have this payment method.");

    var order = new Order
    {
        CustomerId = requestData.UserId,
        UserPaymentMethodId = userPaymentMethod.Id, // ✅ Now storing the correct payment method reference
        IsComplete = false,
        OrderDate = DateTime.UtcNow
    };

    db.Orders.Add(order);
    await db.SaveChangesAsync();

    var orderItems = cart.CartItems.Select(ci => new OrderItem
    {
        OrderId = order.Id,
        ProductId = ci.ProductId,
        Quantity = ci.Quantity
    }).ToList();

    db.OrderItems.AddRange(orderItems);
    db.CartItems.RemoveRange(cart.CartItems);
    db.Carts.Remove(cart);
    await db.SaveChangesAsync();

    return Results.Ok(order);
});


// ✅ Add a Payment Method to a User
app.MapPost("/user-payment-methods", async (BangazonDbContext db, HttpContext context) =>
{
    var requestData = await context.Request.ReadFromJsonAsync<UserPaymentMethodRequest>();
    if (requestData == null) return Results.BadRequest("Invalid request format.");

    var userExists = await db.Users.AnyAsync(u => u.Uid == requestData.UserId);
    if (!userExists) return Results.NotFound("User not found.");

    var paymentOptionExists = await db.PaymentOptions.AnyAsync(po => po.Id == requestData.PaymentOptionId);
    if (!paymentOptionExists) return Results.NotFound("Invalid payment option.");

    var userPaymentMethod = new UserPaymentMethod
    {
        UserId = requestData.UserId,
        PaymentOptionId = requestData.PaymentOptionId
    };

    db.UserPaymentMethods.Add(userPaymentMethod);
    await db.SaveChangesAsync();

    return Results.Created($"/user-payment-methods/{userPaymentMethod.Id}", userPaymentMethod);
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

// ✅ CATEGORY Calls

app.MapGet("/categories", (BangazonDbContext db) =>
{
    return Results.Ok(db.Categories.ToList());
});

app.MapGet("/categories/{categoryId}", (BangazonDbContext db, int categoryId) =>
{
    var category = db.Categories.FirstOrDefault(c => c.Id == categoryId);
    return category != null ? Results.Ok(category) : Results.NotFound();
});

app.MapPost("/categories", async (BangazonDbContext db, Category category) =>
{
    if (string.IsNullOrWhiteSpace(category.Title))
    {
        return Results.BadRequest("Category title is required.");
    }

    db.Categories.Add(category);
    await db.SaveChangesAsync();
    
    return Results.Created($"/categories/{category.Id}", category);
});


app.MapDelete("/categories/{categoryId}", (BangazonDbContext db, int categoryId) =>
{
    var category = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.Id == categoryId);

    if (category == null) return Results.NotFound("Category not found.");

    if (category.Products.Any())
    {
        return Results.BadRequest("Cannot delete category with existing products.");
    }

    db.Categories.Remove(category);
    db.SaveChanges();
    return Results.NoContent();
});


app.MapGet("/products/category/{categoryId}", (BangazonDbContext db, int categoryId) =>
{
    var products = db.Products
        .Where(p => p.CategoryId == categoryId)
        .Include(p => p.Category)
        .ToList();

    return products.Any() ? Results.Ok(products) : Results.NotFound($"No products found in category {categoryId}.");
});


app.Run();


// ✅ Define a DTO (Data Transfer Object) for the Request
public class CartRequest
{
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateCartRequest
{
    public string UserId { get; set; }
    public int ProductId { get; set; }
    public int NewQuantity { get; set; }
}

public class CompleteOrderRequest
{
    public string UserId { get; set; }
    public int PaymentOptionId { get; set; }
}

public class UserPaymentMethodRequest
{
    public string UserId { get; set; }
    public int PaymentOptionId { get; set; }
}
