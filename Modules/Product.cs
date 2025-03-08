using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class Product
{
  public int Id { get; set; }
  public string Name { get; set; }
  public bool IsAvailable { get; set; }
  public decimal Price { get; set; }
  public string Image { get; set; }
  public string Description { get; set; }
  public int Quantity { get; set; }
  public int CategoryId { get; set; }
  public string SellerId { get; set; }
  public List<OrderItem> OrderItems { get; set; }
  public CartItem CartItem { get; set; }
  public Category Category { get; set; }
}