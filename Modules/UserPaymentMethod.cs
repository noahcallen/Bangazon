using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bangazon.Modules;

public class UserPaymentMethod
{
  public int Id { get; set; }

  [Required]
  public string UserId { get; set; }  // ✅ Must match User.Uid

  [ForeignKey("UserId")]
  public User User { get; set; }  // ✅ Correct reference to User

  public int PaymentOptionId { get; set; }

  [ForeignKey("PaymentOptionId")]
  public PaymentOption PaymentOption { get; set; }
}