using System.ComponentModel.DataAnnotations;

namespace Ehandel.Models;

/// <summary>
/// Represents a customer in the shop.
/// The customer also stores password security data.
/// </summary>
public class Customer
{
    public int CustomerId { get; set; }
    [Required, MaxLength(100)] 
    public string Name { get; set; }= string.Empty;
    [MaxLength(100)]
    public string? City { get; set; }

    [Required, MaxLength(300)] 
    public string Email { get; set; }= string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string PasswordSalt { get; set; } = string.Empty;
    public List<Order> Orders { get; set; } = new();
}