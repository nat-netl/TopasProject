using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class SalaryStorageContractTests : BaseStorageContractTest
{
    private SalaryStorageContract _salaryStorageContract = null!;
    private Worker _worker = null!;

    [SetUp]
    public void SetUp()
    {
        _salaryStorageContract = new SalaryStorageContract(TopasDbContext);
        _worker = InsertWorkerToDatabaseAndReturn();
    }

    [TearDown]
    public void TearDown()
    {
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Salaries\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var salary = InsertSalaryToDatabaseAndReturn(_worker.Id, workerSalary: 100);
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        InsertSalaryToDatabaseAndReturn(_worker.Id);
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        Assert.That(list.First().WorkerId, Is.EqualTo(salary.WorkerId));
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _salaryStorageContract.GetList(DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(10));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var salary = CreateModel(_worker.Id);
        _salaryStorageContract.AddElement(salary);
        var fromDb = TopasDbContext.Salaries.FirstOrDefault(x => x.WorkerId == _worker.Id);
        Assert.That(fromDb, Is.Not.Null);
        Assert.That(fromDb!.WorkerSalary, Is.EqualTo(salary.Salary));
    }

    private Worker InsertWorkerToDatabaseAndReturn(string workerFIO = "fio")
    {
        var worker = new Worker { Id = Guid.NewGuid().ToString(), PostId = Guid.NewGuid().ToString(), FIO = workerFIO, BirthDate = DateTime.UtcNow.AddYears(-25), EmploymentDate = DateTime.UtcNow, IsDeleted = false };
        TopasDbContext.Workers.Add(worker);
        TopasDbContext.SaveChanges();
        return worker;
    }

    private Salary InsertSalaryToDatabaseAndReturn(string workerId, double workerSalary = 1, DateTime? salaryDate = null)
    {
        var salary = new Salary { WorkerId = workerId, WorkerSalary = workerSalary, SalaryDate = salaryDate ?? DateTime.UtcNow };
        TopasDbContext.Salaries.Add(salary);
        TopasDbContext.SaveChanges();
        return salary;
    }

    private static SalaryDataModel CreateModel(string workerId, double workerSalary = 1, DateTime? salaryDate = null)
        => new(workerId, salaryDate ?? DateTime.UtcNow, workerSalary);
}
