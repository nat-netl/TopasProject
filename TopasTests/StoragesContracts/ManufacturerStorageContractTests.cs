using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class ManufacturerStorageContractTests : BaseStorageContractTest
{
    private ManufacturerStorageContract _manufacturerStorageContract = null!;

    [SetUp]
    public void SetUp()
    {
        _manufacturerStorageContract = new ManufacturerStorageContract(TopasDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Products\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Manufacturers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3");
        var list = _manufacturerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _manufacturerStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_manufacturerStorageContract.GetElementById(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString());
        _manufacturerStorageContract.AddElement(manufacturer);
        AssertElement(GetManufacturerFromDatabase(manufacturer.Id), manufacturer);
    }

    [Test]
    public void Try_AddElement_WhenHaveRecordWithSameManufacturerName_Test()
    {
        var manufacturer = CreateModel(Guid.NewGuid().ToString(), "name unique");
        InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString(), manufacturerName: manufacturer.ManufacturerName);
        Assert.That(() => _manufacturerStorageContract.AddElement(manufacturer), Throws.TypeOf<ElementExistsException>());
    }

    [Test]
    public void Try_DelElement_WhenNoProducts_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        _manufacturerStorageContract.DelElement(manufacturer.Id);
        Assert.That(GetManufacturerFromDatabase(manufacturer.Id), Is.Null);
    }

    [Test]
    public void Try_DelElement_WhenHaveProducts_Test()
    {
        var manufacturer = InsertManufacturerToDatabaseAndReturn(Guid.NewGuid().ToString());
        TopasDbContext.Products.Add(new Product { Id = Guid.NewGuid().ToString(), ProductName = "name", ManufacturerId = manufacturer.Id, Price = 10, IsDeleted = false });
        TopasDbContext.SaveChanges();
        Assert.That(() => _manufacturerStorageContract.DelElement(manufacturer.Id), Throws.TypeOf<StorageException>());
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn(string id, string manufacturerName = "test", string? prevManufacturerName = "prev", string? prevPrevManufacturerName = "prevPrev")
    {
        var manufacturer = new Manufacturer { Id = id, ManufacturerName = manufacturerName, PrevManufacturerName = prevManufacturerName, PrevPrevManufacturerName = prevPrevManufacturerName };
        TopasDbContext.Manufacturers.Add(manufacturer);
        TopasDbContext.SaveChanges();
        return manufacturer;
    }

    private static void AssertElement(ManufacturerDataModel? actual, Manufacturer expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(actual!.Id, Is.EqualTo(expected.Id));
            Assert.That(actual.ManufacturerName, Is.EqualTo(expected.ManufacturerName));
        });
    }

    private static ManufacturerDataModel CreateModel(string id, string manufacturerName = "test", string? prevManufacturerName = "prev", string? prevPrevManufacturerName = "prevPrev")
        => new(id, manufacturerName, prevManufacturerName, prevPrevManufacturerName);

    private Manufacturer? GetManufacturerFromDatabase(string id) => TopasDbContext.Manufacturers.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Manufacturer? actual, ManufacturerDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.ManufacturerName, Is.EqualTo(expected.ManufacturerName));
    }
}
