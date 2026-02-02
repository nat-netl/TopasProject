using System.ComponentModel.DataAnnotations.Schema;
using TopasContracts.Enums;

namespace TopasDatabase.Models;

internal class Product
{
    public required string Id { get; set; }
    public required string ManufacturerId { get; set; }
    public required string ProductName { get; set; }
    public ProductType ProductType { get; set; }
    public double Price { get; set; }
    public bool IsDeleted { get; set; }
    public string? PrevProductName { get; set; }
    public string? PrevPrevProductName { get; set; }

    public Manufacturer? Manufacturer { get; set; }

    [ForeignKey("ProductId")]
    public List<ProductHistory>? ProductHistories { get; set; }

    [ForeignKey("ProductId")]
    public List<SaleProduct>? SaleProducts { get; set; }
}
