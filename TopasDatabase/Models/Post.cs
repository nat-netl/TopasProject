using TopasContracts.Enums;

namespace TopasDatabase.Models;

internal class Post
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string PostId { get; set; }
    public required string PostName { get; set; }
    public PostType PostType { get; set; }
    public double Salary { get; set; }
    public bool IsActual { get; set; }
    public DateTime ChangeDate { get; set; }
}
