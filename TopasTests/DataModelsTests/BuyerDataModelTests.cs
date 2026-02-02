using TopasContracts.DataModels;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class BuyerDataModelTests
{
    [Test]
    public void IdIsNullOrEmptyTest()
    {
        var buyer = CreateDataModel(null, "fio", "number", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
        buyer = CreateDataModel(string.Empty, "fio", "number", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var buyer = CreateDataModel("id", "fio", "number", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void FIOIsNullOrEmptyTest()
    {
        var buyer = CreateDataModel(Guid.NewGuid().ToString(), null, "number", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
        buyer = CreateDataModel(Guid.NewGuid().ToString(), string.Empty, "number", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PhoneNumberIsNullOrEmptyTest()
    {
        var buyer = CreateDataModel(Guid.NewGuid().ToString(), "fio", null, 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
        buyer = CreateDataModel(Guid.NewGuid().ToString(), "fio", string.Empty, 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PhoneNumberIsIncorrectTest()
    {
        var buyer = CreateDataModel(Guid.NewGuid().ToString(), "fio", "777", 10);
        Assert.That(() => buyer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var buyerId = Guid.NewGuid().ToString();
        var fio = "Fio";
        var phoneNumber = "+7-777-777-77-77";
        var discountSize = 11;
        var buyer = CreateDataModel(buyerId, fio, phoneNumber, discountSize);
        Assert.That(() => buyer.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(buyer.Id, Is.EqualTo(buyerId));
            Assert.That(buyer.FIO, Is.EqualTo(fio));
            Assert.That(buyer.PhoneNumber, Is.EqualTo(phoneNumber));
            Assert.That(buyer.DiscountSize, Is.EqualTo(discountSize));
        });
    }

    private static BuyerDataModel CreateDataModel(string? id, string? fio, string? phoneNumber, double discountSize) =>
        new(id, fio, phoneNumber, discountSize);
}
