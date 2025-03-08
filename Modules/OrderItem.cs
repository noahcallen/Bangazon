using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangazon.Modules;

public class OrderItem
{
  public int Id { get; set; }
  public int OrderId { get; set; }
  public int ProductId { get; set; }
  public int Quantity { get; set; }
  public string SellerId { get; set; }

  [ForeignKey("OrderId")]
  public Order Order { get; set; }  // ✅ Reference to Order

  [ForeignKey("ProductId")]
  public Product Product { get; set; }  // ✅ Reference to Product
}