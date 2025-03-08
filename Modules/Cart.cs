using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class Cart
{
  public int Id { get; set; }

  [Required]
  public string UserId { get; set; }  // ✅ This must match `User.Uid`

  [ForeignKey("UserId")]
  public User User { get; set; }  // ✅ Correct foreign key reference

  public int UserPaymentMethodId { get; set; }

  public List<CartItem> CartItems { get; set; }
}