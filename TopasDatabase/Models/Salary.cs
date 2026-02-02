namespace TopasDatabase.Models;

internal class Salary
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public required string WorkerId { get; set; }
    public double WorkerSalary { get; set; }
    public DateTime SalaryDate { get; set; }

    public Worker? Worker { get; set; }
}
