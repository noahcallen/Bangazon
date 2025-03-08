using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Bangazon.Modules;

public class CartItem
{
  public int Id { get; set; }
  public int CartId { get; set; }
  public int ProductId { get; set; }
  public int Quantity { get; set; }
  public Product Product { get; set; }
}