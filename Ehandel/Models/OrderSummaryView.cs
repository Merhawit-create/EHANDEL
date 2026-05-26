using Microsoft.EntityFrameworkCore;

namespace Ehandel.Models;

[Keyless]
public class OrderSummaryView
{
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string CustomerName { get; set; } = string.Empty;
}