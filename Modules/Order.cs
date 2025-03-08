using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class Order
{
    internal DateTime OrderDate;

    public int Id { get; set; }

  [Required]
  public string CustomerId { get; set; }  // ✅ Must match User.Uid
  

  [ForeignKey("CustomerId")]
  public User User { get; set; }  // ✅ Correct reference to User

  public bool IsComplete { get; set; }
  public int UserPaymentMethodId { get; set; }

  [ForeignKey("UserPaymentMethodId")]
  public UserPaymentMethod UserPaymentMethod { get; set; }


  public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();  // ✅ Ensure non-null list
}