using System.ComponentModel.DataAnnotations.Schema;
using TopasContracts.Enums;

namespace TopasDatabase.Models;

internal class Sale
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string WorkerId { get; set; }
    public string? BuyerId { get; set; }
    public DateTime SaleDate { get; set; }
    public double Sum { get; set; }
    public DiscountType DiscountType { get; set; }
    public double Discount { get; set; }
    public bool IsCancel { get; set; }

    public Worker? Worker { get; set; }
    public Buyer? Buyer { get; set; }

    [ForeignKey("SaleId")]
    public List<SaleProduct>? SaleProducts { get; set; }
}
