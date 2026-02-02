using System.ComponentModel.DataAnnotations.Schema;

namespace TopasDatabase.Models;

internal class Buyer
{
    public required string Id { get; set; }
    public required string FIO { get; set; }
    public required string PhoneNumber { get; set; }
    public double DiscountSize { get; set; }

    [ForeignKey("BuyerId")]
    public List<Sale>? Sales { get; set; }
}
