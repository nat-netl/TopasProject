using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class WorkerStorageContractTests : BaseStorageContractTest
{
    private WorkerStorageContract _workerStorageContract = null!;

    [SetUp]
    public void SetUp()
    {
        _workerStorageContract = new WorkerStorageContract(TopasDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 1");
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 2");
        InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString(), "fio 3");
        var list = _workerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == worker.Id), worker);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _workerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_workerStorageContract.GetElementById(worker.Id), worker);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString());
        _workerStorageContract.AddElement(worker);
        AssertElement(GetWorkerFromDatabase(worker.Id), worker);
    }

    [Test]
    public void Try_UpdElement_Test()
    {
        var worker = CreateModel(Guid.NewGuid().ToString(), "New Fio");
        InsertWorkerToDatabaseAndReturn(worker.Id);
        _workerStorageContract.UpdElement(worker);
        AssertElement(GetWorkerFromDatabase(worker.Id), worker);
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var worker = InsertWorkerToDatabaseAndReturn(Guid.NewGuid().ToString());
        _workerStorageContract.DelElement(worker.Id);
        var element = GetWorkerFromDatabase(worker.Id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element!.IsDeleted, Is.True);
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _workerStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    private Worker InsertWorkerToDatabaseAndReturn(string id, string fio = "test", string? postId = null, DateTime? birthDate = null, DateTime? employmentDate = null, bool isDeleted = false)
    {
        var worker = new Worker { Id = id, FIO = fio, PostId = postId ?? Guid.NewGuid().ToString(), BirthDate = birthDate ?? DateTime.UtcNow.AddYears(-20), EmploymentDate = employmentDate ?? DateTime.UtcNow, IsDeleted = isDeleted };
        TopasDbContext.Workers.Add(worker);
        TopasDbContext.SaveChanges();
        return worker;
    }

    private static void AssertElement(WorkerDataModel? actual, Worker expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
    }

    private static WorkerDataModel CreateModel(string id, string fio = "fio", string? postId = null, DateTime? birthDate = null, DateTime? employmentDate = null, bool isDeleted = false)
        => new(id, fio, postId ?? Guid.NewGuid().ToString(), birthDate ?? DateTime.UtcNow.AddYears(-20), employmentDate ?? DateTime.UtcNow, isDeleted);

    private Worker? GetWorkerFromDatabase(string id) => TopasDbContext.Workers.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Worker? actual, WorkerDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.FIO, Is.EqualTo(expected.FIO));
    }
}
