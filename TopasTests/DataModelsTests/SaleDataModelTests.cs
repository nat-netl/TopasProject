using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class SaleDataModelTests
{
    [Test]
    public void IdIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(null, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var sale = CreateDataModel("id", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void WorkerIdIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), null, Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(Guid.NewGuid().ToString(), string.Empty, Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void WorkerIdIsNotGuidTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), "workerId", Guid.NewGuid().ToString(), DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void BuyerIdIsNotGuidTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "buyerId", DiscountType.OnSale, false, CreateSubDataModel());
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductsIsNullOrEmptyTest()
    {
        var sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DiscountType.OnSale, false, null);
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
        sale = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), DiscountType.OnSale, false, []);
        Assert.That(() => sale.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void CalcSumAndDiscountTest()
    {
        var saleId = Guid.NewGuid().ToString();
        var workerId = Guid.NewGuid().ToString();
        var buyerId = Guid.NewGuid().ToString();
        var products = new List<SaleProductDataModel>()
        {
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 2, 1.1),
            new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1, 1.3)
        };
        var isCancel = false;
        var totalSum = products.Sum(x => x.Price * x.Count);
        var saleNone = CreateDataModel(saleId, workerId, buyerId, DiscountType.None, isCancel, products);
        Assert.Multiple(() =>
        {
            Assert.That(saleNone.Sum, Is.EqualTo(totalSum));
            Assert.That(saleNone.Discount, Is.EqualTo(0));
        });
        var saleOnSale = CreateDataModel(saleId, workerId, buyerId, DiscountType.OnSale, isCancel, products);
        Assert.Multiple(() =>
        {
            Assert.That(saleOnSale.Sum, Is.EqualTo(totalSum));
            Assert.That(saleOnSale.Discount, Is.EqualTo(totalSum * 0.1));
        });
        var saleRegularCustomer = CreateDataModel(saleId, workerId, buyerId, DiscountType.RegularCustomer, isCancel, products);
        Assert.Multiple(() =>
        {
            Assert.That(saleRegularCustomer.Sum, Is.EqualTo(totalSum));
            Assert.That(saleRegularCustomer.Discount, Is.EqualTo(totalSum * 0.5));
        });
        var saleCertificate = CreateDataModel(saleId, workerId, buyerId, DiscountType.Certificate, isCancel, products);
        Assert.Multiple(() =>
        {
            Assert.That(saleCertificate.Sum, Is.EqualTo(totalSum));
            Assert.That(saleCertificate.Discount, Is.EqualTo(totalSum * 0.3));
        });
        var saleMulty = CreateDataModel(saleId, workerId, buyerId, DiscountType.Certificate | DiscountType.RegularCustomer, isCancel, products);
        Assert.Multiple(() =>
        {
            Assert.That(saleMulty.Sum, Is.EqualTo(totalSum));
            Assert.That(saleMulty.Discount, Is.EqualTo(totalSum * 0.8));
        });
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var saleId = Guid.NewGuid().ToString();
        var workerId = Guid.NewGuid().ToString();
        var buyerId = Guid.NewGuid().ToString();
        var discountType = DiscountType.Certificate;
        var isCancel = true;
        var products = CreateSubDataModel();
        var sale = CreateDataModel(saleId, workerId, buyerId, discountType, isCancel, products);
        Assert.That(() => sale.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(sale.Id, Is.EqualTo(saleId));
            Assert.That(sale.WorkerId, Is.EqualTo(workerId));
            Assert.That(sale.BuyerId, Is.EqualTo(buyerId));
            Assert.That(sale.DiscountType, Is.EqualTo(discountType));
            Assert.That(sale.IsCancel, Is.EqualTo(isCancel));
            Assert.That(sale.Products, Is.EquivalentTo(products));
        });
    }

    private static SaleDataModel CreateDataModel(string? id, string? workerId, string? buyerId, DiscountType discountType, bool isCancel, List<SaleProductDataModel>? products) =>
        new(id, workerId, buyerId, discountType, isCancel, products);

    private static List<SaleProductDataModel> CreateSubDataModel() =>
        [new(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1, 1.1)];
}
