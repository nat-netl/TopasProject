using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class ProductStorageContractTests : BaseStorageContractTest
{
    private ProductStorageContract _productStorageContract = null!;
    private Manufacturer _manufacturer = null!;

    [SetUp]
    public void SetUp()
    {
        _productStorageContract = new ProductStorageContract(TopasDbContext);
        _manufacturer = InsertManufacturerToDatabaseAndReturn();
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
        var product = InsertProductToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 1");
        InsertProductToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 2");
        InsertProductToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, "name 3");
        var list = _productStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == product.Id), product);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _productStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var product = InsertProductToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id);
        AssertElement(_productStorageContract.GetElementById(product.Id), product);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var product = CreateModel(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: false);
        _productStorageContract.AddElement(product);
        AssertElement(GetProductFromDatabaseById(product.Id), product);
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var product = InsertProductToDatabaseAndReturn(Guid.NewGuid().ToString(), _manufacturer.Id, isDeleted: false);
        _productStorageContract.DelElement(product.Id);
        var element = GetProductFromDatabaseById(product.Id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element!.IsDeleted, Is.True);
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn(string manufacturerName = "name")
    {
        var manufacturer = new Manufacturer { Id = Guid.NewGuid().ToString(), ManufacturerName = manufacturerName };
        TopasDbContext.Manufacturers.Add(manufacturer);
        TopasDbContext.SaveChanges();
        return manufacturer;
    }

    private Product InsertProductToDatabaseAndReturn(string id, string manufacturerId, string productName = "test", ProductType productType = ProductType.Ring, double price = 1, bool isDeleted = false)
    {
        var product = new Product { Id = id, ManufacturerId = manufacturerId, ProductName = productName, ProductType = productType, Price = price, IsDeleted = isDeleted };
        TopasDbContext.Products.Add(product);
        TopasDbContext.SaveChanges();
        return product;
    }

    private static void AssertElement(ProductDataModel? actual, Product expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.ProductName, Is.EqualTo(expected.ProductName));
    }

    private static ProductDataModel CreateModel(string id, string manufacturerId, string productName = "test", ProductType productType = ProductType.Ring, double price = 1, bool isDeleted = false)
        => new(id, productName, productType, manufacturerId, price, isDeleted);

    private Product? GetProductFromDatabaseById(string id) => TopasDbContext.Products.FirstOrDefault(x => x.Id == id);

    private static void AssertElement(Product? actual, ProductDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.Id));
        Assert.That(actual.ProductName, Is.EqualTo(expected.ProductName));
    }
}
