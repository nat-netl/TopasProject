using TopasContracts.DataModels;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class SalaryDataModelTests
{
    [Test]
    public void WorkerIdIsEmptyTest()
    {
        var salary = CreateDataModel(null, DateTime.Now, 10);
        Assert.That(() => salary.Validate(), Throws.TypeOf<ValidationException>());
        salary = CreateDataModel(string.Empty, DateTime.Now, 10);
        Assert.That(() => salary.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void WorkerIdIsNotGuidTest()
    {
        var salary = CreateDataModel("workerId", DateTime.Now, 10);
        Assert.That(() => salary.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PriceIsLessOrZeroTest()
    {
        var salary = CreateDataModel(Guid.NewGuid().ToString(), DateTime.Now, 0);
        Assert.That(() => salary.Validate(), Throws.TypeOf<ValidationException>());
        salary = CreateDataModel(Guid.NewGuid().ToString(), DateTime.Now, -10);
        Assert.That(() => salary.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var workerId = Guid.NewGuid().ToString();
        var salaryDate = DateTime.Now.AddDays(-3).AddMinutes(-5);
        var workerSalary = 10;
        var salary = CreateDataModel(workerId, salaryDate, workerSalary);
        Assert.That(() => salary.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(salary.WorkerId, Is.EqualTo(workerId));
            Assert.That(salary.SalaryDate, Is.EqualTo(salaryDate));
            Assert.That(salary.Salary, Is.EqualTo(workerSalary));
        });
    }

    private static SalaryDataModel CreateDataModel(string? workerId, DateTime salaryDate, double workerSalary) =>
        new(workerId, salaryDate, workerSalary);
}
