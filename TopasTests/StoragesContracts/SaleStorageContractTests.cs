using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class SaleStorageContractTests : BaseStorageContractTest
{
    private SaleStorageContract _saleStorageContract = null!;
    private Buyer _buyer = null!;
    private Worker _worker = null!;
    private Product _product = null!;
    private Manufacturer _manufacturer = null!;

    [SetUp]
    public void SetUp()
    {
        _saleStorageContract = new SaleStorageContract(TopasDbContext);
        _manufacturer = InsertManufacturerToDatabaseAndReturn();
        _buyer = InsertBuyerToDatabaseAndReturn();
        _worker = InsertWorkerToDatabaseAndReturn();
        _product = InsertProductToDatabaseAndReturn();
    }

    [TearDown]
    public void TearDown()
    {
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"SaleProducts\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Sales\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Buyers\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Workers\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Products\" CASCADE;");
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Manufacturers\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, products: [(_product.Id, 1, 1.2)]);
        InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, products: [(_product.Id, 5, 1.2)]);
        var list = _saleStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        Assert.That(list.First(x => x.Id == sale.Id).WorkerId, Is.EqualTo(sale.WorkerId));
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _saleStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, products: [(_product.Id, 1, 1.2)]);
        var found = _saleStorageContract.GetElementById(sale.Id);
        Assert.That(found, Is.Not.Null);
        Assert.That(found!.Id, Is.EqualTo(sale.Id));
        Assert.That(found.Products, Has.Count.EqualTo(1));
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var sale = CreateModel(Guid.NewGuid().ToString(), _worker.Id, _buyer.Id, DiscountType.RegularCustomer, false, [_product.Id]);
        _saleStorageContract.AddElement(sale);
        var fromDb = TopasDbContext.Sales.Include(x => x.SaleProducts).FirstOrDefault(x => x.Id == sale.Id);
        Assert.That(fromDb, Is.Not.Null);
        Assert.That(fromDb!.SaleProducts, Has.Count.EqualTo(1));
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var sale = InsertSaleToDatabaseAndReturn(_worker.Id, _buyer.Id, products: [(_product.Id, 1, 1.2)], isCancel: false);
        _saleStorageContract.DelElement(sale.Id);
        var element = TopasDbContext.Sales.FirstOrDefault(x => x.Id == sale.Id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element!.IsCancel, Is.True);
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _saleStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    private Buyer InsertBuyerToDatabaseAndReturn(string fio = "test", string phoneNumber = "+7-777-777-77-77")
    {
        var buyer = new Buyer { Id = Guid.NewGuid().ToString(), FIO = fio, PhoneNumber = phoneNumber, DiscountSize = 10 };
        TopasDbContext.Buyers.Add(buyer);
        TopasDbContext.SaveChanges();
        return buyer;
    }

    private Worker InsertWorkerToDatabaseAndReturn(string fio = "test")
    {
        var worker = new Worker { Id = Guid.NewGuid().ToString(), FIO = fio, PostId = Guid.NewGuid().ToString(), BirthDate = DateTime.UtcNow.AddYears(-25), EmploymentDate = DateTime.UtcNow };
        TopasDbContext.Workers.Add(worker);
        TopasDbContext.SaveChanges();
        return worker;
    }

    private Manufacturer InsertManufacturerToDatabaseAndReturn()
    {
        var manufacturer = new Manufacturer { Id = Guid.NewGuid().ToString(), ManufacturerName = "name" };
        TopasDbContext.Manufacturers.Add(manufacturer);
        TopasDbContext.SaveChanges();
        return manufacturer;
    }

    private Product InsertProductToDatabaseAndReturn(string productName = "test", double price = 1)
    {
        var product = new Product { Id = Guid.NewGuid().ToString(), ManufacturerId = _manufacturer.Id, ProductName = productName, ProductType = TopasContracts.Enums.ProductType.Ring, Price = price, IsDeleted = false };
        TopasDbContext.Products.Add(product);
        TopasDbContext.SaveChanges();
        return product;
    }

    private Sale InsertSaleToDatabaseAndReturn(string workerId, string? buyerId, DateTime? saleDate = null, double sum = 1, DiscountType discountType = DiscountType.OnSale, double discount = 0, bool isCancel = false, List<(string, int, double)>? products = null)
    {
        var sale = new Sale { WorkerId = workerId, BuyerId = buyerId, SaleDate = saleDate ?? DateTime.UtcNow, Sum = sum, DiscountType = discountType, Discount = discount, IsCancel = isCancel, SaleProducts = [] };
        TopasDbContext.Sales.Add(sale);
        TopasDbContext.SaveChanges();
        if (products is not null)
        {
            foreach (var (productId, count, price) in products)
            {
                TopasDbContext.SaleProducts.Add(new SaleProduct { SaleId = sale.Id, ProductId = productId, Count = count, Price = price });
            }
            TopasDbContext.SaveChanges();
        }
        return sale;
    }

    private static SaleDataModel CreateModel(string id, string workerId, string? buyerId, DiscountType discountType, bool isCancel, List<string> productIds)
    {
        var products = productIds.Select(x => new SaleProductDataModel(id, x, 1, 1.1)).ToList();
        return new(id, workerId, buyerId, discountType, isCancel, products);
    }
}
