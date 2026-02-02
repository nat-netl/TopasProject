namespace TopasDatabase.Models;

internal class ProductHistory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string ProductId { get; set; }
    public double OldPrice { get; set; }
    public DateTime ChangeDate { get; set; } = DateTime.UtcNow;

    public Product? Product { get; set; }
}
