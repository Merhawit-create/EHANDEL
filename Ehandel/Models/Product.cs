using System.ComponentModel.DataAnnotations;

namespace Ehandel.Models;

public class Product
{
    public int ProductId { get; set; }
    [Required, MaxLength(100)] 
    public string? ProductName { get; set; } = string.Empty;
    [Required]
    public decimal ProductPrice { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    [MaxLength(100)]
    public string? Description { get; set; }
    public List<OrderRow> OrderRows { get; set; } = new();

}