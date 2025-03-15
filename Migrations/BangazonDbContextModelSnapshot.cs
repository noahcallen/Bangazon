﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bangazon.Migrations
{
    [DbContext(typeof(BangazonDbContext))]
    partial class BangazonDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Bangazon.Modules.Cart", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserPaymentMethodId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("Carts");
                });

            modelBuilder.Entity("Bangazon.Modules.CartItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CartId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CartId");

                    b.HasIndex("ProductId")
                        .IsUnique();

                    b.ToTable("CartItems");
                });

            modelBuilder.Entity("Bangazon.Modules.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("CustomerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsComplete")
                        .HasColumnType("boolean");

                    b.Property<int>("UserPaymentMethodId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CustomerId");

                    b.HasIndex("UserPaymentMethodId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("Bangazon.Modules.OrderItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("OrderId")
                        .HasColumnType("integer");

                    b.Property<int>("ProductId")
                        .HasColumnType("integer");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.Property<string>("SellerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("Bangazon.Modules.PaymentOption", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("PaymentOptions");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Type = "Credit Card"
                        },
                        new
                        {
                            Id = 2,
                            Type = "Apple Pay"
                        },
                        new
                        {
                            Id = 3,
                            Type = "Google Pay"
                        },
                        new
                        {
                            Id = 4,
                            Type = "PayPal"
                        });
                });

            modelBuilder.Entity("Bangazon.Modules.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("CategoryId")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Image")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsAvailable")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Price")
                        .HasColumnType("numeric");

                    b.Property<int>("Quantity")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Products");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CategoryId = 1,
                            Description = "Compact and fuel-efficient sedan.",
                            Image = "civic.jpg",
                            IsAvailable = true,
                            Name = "Honda Civic",
                            Price = 24000.99m,
                            Quantity = 8
                        },
                        new
                        {
                            Id = 2,
                            CategoryId = 1,
                            Description = "Reliable and affordable compact sedan.",
                            Image = "corolla.jpg",
                            IsAvailable = true,
                            Name = "Toyota Corolla",
                            Price = 22000.99m,
                            Quantity = 10
                        },
                        new
                        {
                            Id = 3,
                            CategoryId = 1,
                            Description = "High-performance luxury electric sedan.",
                            Image = "models.jpg",
                            IsAvailable = true,
                            Name = "Tesla Model S",
                            Price = 79999.99m,
                            Quantity = 3
                        },
                        new
                        {
                            Id = 4,
                            CategoryId = 2,
                            Description = "Spacious SUV for family trips.",
                            Image = "explorer.jpg",
                            IsAvailable = true,
                            Name = "Ford Explorer",
                            Price = 35000.99m,
                            Quantity = 5
                        },
                        new
                        {
                            Id = 5,
                            CategoryId = 2,
                            Description = "Full-size SUV with premium features.",
                            Image = "tahoe.jpg",
                            IsAvailable = true,
                            Name = "Chevrolet Tahoe",
                            Price = 48000.99m,
                            Quantity = 4
                        },
                        new
                        {
                            Id = 6,
                            CategoryId = 2,
                            Description = "Off-road capable and luxury interior.",
                            Image = "grandcherokee.jpg",
                            IsAvailable = true,
                            Name = "Jeep Grand Cherokee",
                            Price = 42000.99m,
                            Quantity = 7
                        },
                        new
                        {
                            Id = 7,
                            CategoryId = 3,
                            Description = "Powerful truck for all your needs.",
                            Image = "f150.jpg",
                            IsAvailable = true,
                            Name = "Ford F-150",
                            Price = 42000.99m,
                            Quantity = 6
                        },
                        new
                        {
                            Id = 8,
                            CategoryId = 3,
                            Description = "Durable truck with a smooth ride.",
                            Image = "ram1500.jpg",
                            IsAvailable = true,
                            Name = "Ram 1500",
                            Price = 45000.99m,
                            Quantity = 5
                        },
                        new
                        {
                            Id = 9,
                            CategoryId = 3,
                            Description = "Strong towing capacity and reliability.",
                            Image = "silverado.jpg",
                            IsAvailable = true,
                            Name = "Chevrolet Silverado",
                            Price = 46000.99m,
                            Quantity = 8
                        });
                });

            modelBuilder.Entity("Bangazon.Modules.User", b =>
                {
                    b.Property<string>("Uid")
                        .HasColumnType("text");

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("City")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Zip")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Uid");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Uid = "BCyBV6WJpTZcPG0b0WMfr8vJM1B3",
                            Address = "123 Oak Street",
                            City = "Houston",
                            Email = "michaelj@example.com",
                            FirstName = "Michael",
                            LastName = "Johnson",
                            State = "TX",
                            Zip = "77001"
                        },
                        new
                        {
                            Uid = "LoBA4EB98KfPtTZ7t8hE2xlbURw1",
                            Address = "456 Pine Street",
                            City = "Chicago",
                            Email = "emilyd@example.com",
                            FirstName = "Emily",
                            LastName = "Davis",
                            State = "IL",
                            Zip = "60601"
                        });
                });

            modelBuilder.Entity("Bangazon.Modules.UserPaymentMethod", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("PaymentOptionId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("PaymentOptionId");

                    b.HasIndex("UserId");

                    b.ToTable("UserPaymentMethods");
                });

            modelBuilder.Entity("Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Title = "Sedan"
                        },
                        new
                        {
                            Id = 2,
                            Title = "SUV"
                        },
                        new
                        {
                            Id = 3,
                            Title = "Truck"
                        });
                });

            modelBuilder.Entity("Bangazon.Modules.Cart", b =>
                {
                    b.HasOne("Bangazon.Modules.User", "User")
                        .WithOne()
                        .HasForeignKey("Bangazon.Modules.Cart", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bangazon.Modules.CartItem", b =>
                {
                    b.HasOne("Bangazon.Modules.Cart", null)
                        .WithMany("CartItems")
                        .HasForeignKey("CartId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bangazon.Modules.Product", "Product")
                        .WithOne("CartItem")
                        .HasForeignKey("Bangazon.Modules.CartItem", "ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Bangazon.Modules.Order", b =>
                {
                    b.HasOne("Bangazon.Modules.User", "User")
                        .WithMany("Orders")
                        .HasForeignKey("CustomerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bangazon.Modules.UserPaymentMethod", "UserPaymentMethod")
                        .WithMany()
                        .HasForeignKey("UserPaymentMethodId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");

                    b.Navigation("UserPaymentMethod");
                });

            modelBuilder.Entity("Bangazon.Modules.OrderItem", b =>
                {
                    b.HasOne("Bangazon.Modules.Order", "Order")
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bangazon.Modules.Product", "Product")
                        .WithMany("OrderItems")
                        .HasForeignKey("ProductId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Order");

                    b.Navigation("Product");
                });

            modelBuilder.Entity("Bangazon.Modules.Product", b =>
                {
                    b.HasOne("Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Bangazon.Modules.UserPaymentMethod", b =>
                {
                    b.HasOne("Bangazon.Modules.PaymentOption", "PaymentOption")
                        .WithMany("UserPaymentMethods")
                        .HasForeignKey("PaymentOptionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Bangazon.Modules.User", "User")
                        .WithMany("UserPaymentMethods")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("PaymentOption");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Bangazon.Modules.Cart", b =>
                {
                    b.Navigation("CartItems");
                });

            modelBuilder.Entity("Bangazon.Modules.Order", b =>
                {
                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Bangazon.Modules.PaymentOption", b =>
                {
                    b.Navigation("UserPaymentMethods");
                });

            modelBuilder.Entity("Bangazon.Modules.Product", b =>
                {
                    b.Navigation("CartItem")
                        .IsRequired();

                    b.Navigation("OrderItems");
                });

            modelBuilder.Entity("Bangazon.Modules.User", b =>
                {
                    b.Navigation("Orders");

                    b.Navigation("UserPaymentMethods");
                });

            modelBuilder.Entity("Category", b =>
                {
                    b.Navigation("Products");
                });
#pragma warning restore 612, 618
        }
    }
}
