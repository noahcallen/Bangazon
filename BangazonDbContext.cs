using Microsoft.EntityFrameworkCore;
using Bangazon.Modules;
using Microsoft.EntityFrameworkCore;
using Bangazon.Modules; 

public class BangazonDbContext : DbContext
{
  public DbSet<User> Users { get; set; }
  public DbSet<Category> Categories { get; set; }
  public DbSet<PaymentOption> PaymentOptions { get; set; }
  public DbSet<UserPaymentMethod> UserPaymentMethods { get; set; }
  public DbSet<Product> Products { get; set; }
  public DbSet<CartItem> CartItems { get; set; }
  public DbSet<Cart> Carts { get; set; }
  public DbSet<Order> Orders { get; set; }
  public DbSet<OrderItem> OrderItems { get; set; }

  public BangazonDbContext(DbContextOptions<BangazonDbContext> context) : base(context)
  {
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<User>()
        .HasKey(u => u.Uid);  

    modelBuilder.Entity<Cart>()
        .HasOne(c => c.User)
        .WithOne()
        .HasForeignKey<Cart>(c => c.UserId)
        .HasPrincipalKey<User>(u => u.Uid)  
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany(u => u.Orders)
        .HasForeignKey(o => o.CustomerId)
        .HasPrincipalKey(u => u.Uid);  

    modelBuilder.Entity<UserPaymentMethod>()
        .HasOne(upm => upm.User)
        .WithMany(u => u.UserPaymentMethods)
        .HasForeignKey(upm => upm.UserId)
        .HasPrincipalKey(u => u.Uid);  

     modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Title = "Sedan" },
            new Category { Id = 2, Title = "SUV" },
            new Category { Id = 3, Title = "Truck" }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Uid = "BCyBV6WJpTZcPG0b0WMfr8vJM1B3", FirstName = "Michael", LastName = "Johnson", Email = "michaelj@example.com", Address = "123 Oak Street", City = "Houston", State = "TX", Zip = "77001" },
            new User { Uid = "LoBA4EB98KfPtTZ7t8hE2xlbURw1", FirstName = "Emily", LastName = "Davis", Email = "emilyd@example.com", Address = "456 Pine Street", City = "Chicago", State = "IL", Zip = "60601" }
        );

    

        modelBuilder.Entity<Product>().HasData(
        // 🚗 Sedans
        new Product { Id = 1, Name = "Honda Civic", IsAvailable = true, Price = 24000.99m, Image = "civic.jpg", Description = "Compact and fuel-efficient sedan.", Quantity = 8, CategoryId = 1},
        new Product { Id = 2, Name = "Toyota Corolla", IsAvailable = true, Price = 22000.99m, Image = "corolla.jpg", Description = "Reliable and affordable compact sedan.", Quantity = 10, CategoryId = 1},
        new Product { Id = 3, Name = "Tesla Model S", IsAvailable = true, Price = 79999.99m, Image = "models.jpg", Description = "High-performance luxury electric sedan.", Quantity = 3, CategoryId = 1},
        
        // 🚙 SUVs
        new Product { Id = 4, Name = "Ford Explorer", IsAvailable = true, Price = 35000.99m, Image = "explorer.jpg", Description = "Spacious SUV for family trips.", Quantity = 5, CategoryId = 2},
        new Product { Id = 5, Name = "Chevrolet Tahoe", IsAvailable = true, Price = 48000.99m, Image = "tahoe.jpg", Description = "Full-size SUV with premium features.", Quantity = 4, CategoryId = 2},
        new Product { Id = 6, Name = "Jeep Grand Cherokee", IsAvailable = true, Price = 42000.99m, Image = "grandcherokee.jpg", Description = "Off-road capable and luxury interior.", Quantity = 7, CategoryId = 2},

        // 🚛 Trucks
        new Product { Id = 7, Name = "Ford F-150", IsAvailable = true, Price = 42000.99m, Image = "f150.jpg", Description = "Powerful truck for all your needs.", Quantity = 6, CategoryId = 3},
        new Product { Id = 8, Name = "Ram 1500", IsAvailable = true, Price = 45000.99m, Image = "ram1500.jpg", Description = "Durable truck with a smooth ride.", Quantity = 5, CategoryId = 3},
        new Product { Id = 9, Name = "Chevrolet Silverado", IsAvailable = true, Price = 46000.99m, Image = "silverado.jpg", Description = "Strong towing capacity and reliability.", Quantity = 8, CategoryId = 3}

    );


        modelBuilder.Entity<PaymentOption>().HasData(
            new PaymentOption { Id = 1, Type = "Credit Card" },
            new PaymentOption { Id = 2, Type = "Apple Pay" },
            new PaymentOption { Id = 3, Type = "Google Pay" },
            new PaymentOption { Id = 4, Type = "PayPal" }
        );
    }
}
