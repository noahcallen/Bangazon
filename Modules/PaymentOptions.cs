using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class PaymentOption
{
  public int Id { get; set; }

  [Required]
  public string Type { get; set; }  // ✅ Payment type (e.g., "Credit Card", "PayPal")

  public List<UserPaymentMethod> UserPaymentMethods { get; set; } = new List<UserPaymentMethod>();
}