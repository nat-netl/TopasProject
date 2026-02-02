using System.ComponentModel.DataAnnotations.Schema;

namespace TopasDatabase.Models;

internal class Worker
{
    public required string Id { get; set; }
    public required string FIO { get; set; }
    public required string PostId { get; set; }
    public DateTime BirthDate { get; set; }
    public DateTime EmploymentDate { get; set; }
    public bool IsDeleted { get; set; }

    [ForeignKey("WorkerId")]
    public List<Salary>? Salaries { get; set; }

    [ForeignKey("WorkerId")]
    public List<Sale>? Sales { get; set; }
}
