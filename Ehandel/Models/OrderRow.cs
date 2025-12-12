namespace Ehandel.Models;

public class OrderRow
{
    public int OrderRowId { get; set; }
  
    //fk
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    //FK
    public int OrderId { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    // public decimal LinePrice{ get; set;}
}