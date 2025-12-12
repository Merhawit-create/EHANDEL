using System.ComponentModel.DataAnnotations;

namespace Ehandel.Models;

public class Category
{
    public int CategoryId { get; set; }
    [Required,MaxLength(100)]
    
    public string Name { get; set; } = string.Empty;

    public List<Product> Products { get; set; } = new();
}