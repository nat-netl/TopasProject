using TopasContracts.DataModels;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class ProductHistoryDataModelTests
{
    [Test]
    public void ProductIdIsNullOrEmptyTest()
    {
        var product = CreateDataModel(null, 10);
        Assert.That(() => product.Validate(), Throws.TypeOf<ValidationException>());
        product = CreateDataModel(string.Empty, 10);
        Assert.That(() => product.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void ProductIdIsNotGuidTest()
    {
        var product = CreateDataModel("id", 10);
        Assert.That(() => product.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void OldPriceIsLessOrZeroTest()
    {
        var product = CreateDataModel(Guid.NewGuid().ToString(), 0);
        Assert.That(() => product.Validate(), Throws.TypeOf<ValidationException>());
        product = CreateDataModel(Guid.NewGuid().ToString(), -10);
        Assert.That(() => product.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var productId = Guid.NewGuid().ToString();
        var oldPrice = 10;
        var productHistory = CreateDataModel(productId, oldPrice);
        Assert.That(() => productHistory.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(productHistory.ProductId, Is.EqualTo(productId));
            Assert.That(productHistory.OldPrice, Is.EqualTo(oldPrice));
            Assert.That(productHistory.ChangeDate, Is.LessThan(DateTime.UtcNow));
            Assert.That(productHistory.ChangeDate, Is.GreaterThan(DateTime.UtcNow.AddMinutes(-1)));
        });
    }

    private static ProductHistoryDataModel CreateDataModel(string? productId, double oldPrice) =>
        new(productId, oldPrice);
}
