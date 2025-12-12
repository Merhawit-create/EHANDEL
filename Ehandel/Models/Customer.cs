using System.ComponentModel.DataAnnotations;

namespace Ehandel.Models;

public class Customer
{
    public int CustomerId { get; set; }
    [Required, MaxLength(100)] 
    public string Name { get; set; }= string.Empty;
    [MaxLength(100)]
    public string? City { get; set; }

    [Required, MaxLength(100)] 
    public string Email { get; set; }= string.Empty;

    public List<Order> Orders { get; set; } = new();
}