using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class User
{
  [Key]
  public string Uid { get; set; }  // ✅ Firebase UID as primary key

  [Required]
  public string FirstName { get; set; }

  [Required]
  public string LastName { get; set; }

  [Required, EmailAddress]
  public string Email { get; set; }
  public string Address { get; set; }
  public string City { get; set; }
  public string State { get; set; }
  public string Zip { get; set; }

  public List<Order> Orders { get; set; } = new List<Order>();  // ✅ Ensure non-null List
  public List<UserPaymentMethod> UserPaymentMethods { get; set; } = new List<UserPaymentMethod>();
}