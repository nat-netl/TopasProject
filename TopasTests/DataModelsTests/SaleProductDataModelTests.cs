using TopasContracts.DataModels;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class SaleProductDataModelTests
{
    [Test]
    public void SaleIdIsNullOrEmptyTest()
    {
        var saleProduct = CreateDataModel(null, Guid.NewGuid().ToString(), 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
        saleProduct = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void SaleIdIsNotGuidTest()
    {
        var saleProduct = CreateDataModel("saleId", Guid.NewGuid().ToString(), 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNullOrEmptyTest()
    {
        var saleProduct = CreateDataModel(Guid.NewGuid().ToString(), null, 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
        saleProduct = CreateDataModel(string.Empty, Guid.NewGuid().ToString(), 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNotGuidTest()
    {
        var saleProduct = CreateDataModel(Guid.NewGuid().ToString(), "productId", 10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void CountIsLessOrZeroTest()
    {
        var saleProduct = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 0, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
        saleProduct = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), -10, 1.1);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PriceIsLessOrZeroTest()
    {
        var saleProduct = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1, 0);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
        saleProduct = CreateDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 1, -10);
        Assert.That(() => saleProduct.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var saleId = Guid.NewGuid().ToString();
        var productId = Guid.NewGuid().ToString();
        var count = 10;
        var price = 1.2;
        var saleProduct = CreateDataModel(saleId, productId, count, price);
        Assert.That(() => saleProduct.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(saleProduct.SaleId, Is.EqualTo(saleId));
            Assert.That(saleProduct.ProductId, Is.EqualTo(productId));
            Assert.That(saleProduct.Count, Is.EqualTo(count));
            Assert.That(saleProduct.Price, Is.EqualTo(price));
        });
    }

    private static SaleProductDataModel CreateDataModel(string? saleId, string? productId, int count, double price) =>
        new(saleId, productId, count, price);
}
