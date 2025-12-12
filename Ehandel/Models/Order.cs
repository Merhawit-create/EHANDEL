using System.ComponentModel.DataAnnotations;

namespace Ehandel.Models;

public class Order
{
    //PK
   
    public int OrderId{ get; set; }
    [Required]
    public DateTime OrderDate { get; set; }
    [Required, MaxLength(50)]
    public string Status { get; set; }
    [Required]
    public decimal TotalAmount { get; set; }

    public int CustomerId { get; set; } 
    public Customer? Customer { get; set; }
  
    public List<OrderRow> OrderRows { get; set; } = new();
}