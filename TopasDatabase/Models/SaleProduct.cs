namespace TopasDatabase.Models;

internal class SaleProduct
{
    public required string SaleId { get; set; }
    public required string ProductId { get; set; }
    public int Count { get; set; }
    public double Price { get; set; }

    public Sale? Sale { get; set; }
    public Product? Product { get; set; }
}
