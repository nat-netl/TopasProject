using TopasBusinessLogic.Implementations;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TopasTests.BusinessLogicsContractsTests;

[TestFixture]
internal class BuyerBusinessLogicContractTests
{
    private BuyerBusinessLogicContract _buyerBusinessLogicContract = null!;
    private Mock<IBuyerStorageContract> _buyerStorageContract = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _buyerStorageContract = new Mock<IBuyerStorageContract>();
        _buyerBusinessLogicContract = new BuyerBusinessLogicContract(_buyerStorageContract.Object, new Mock<ILogger>().Object);
    }

    [TearDown]
    public void TearDown()
    {
        _buyerStorageContract.Reset();
    }

    [Test]
    public void GetAllBuyers_ReturnListOfRecords_Test()
    {
        var listOriginal = new List<BuyerDataModel>
        {
            new(Guid.NewGuid().ToString(), "fio 1", "+7-111-111-11-11", 0),
            new(Guid.NewGuid().ToString(), "fio 2", "+7-555-444-33-23", 10),
            new(Guid.NewGuid().ToString(), "fio 3", "+7-777-777-7777", 0),
        };
        _buyerStorageContract.Setup(x => x.GetList()).Returns(listOriginal);
        var list = _buyerBusinessLogicContract.GetAllBuyers();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
    }

    [Test]
    public void GetAllBuyers_ReturnEmptyList_Test()
    {
        _buyerStorageContract.Setup(x => x.GetList()).Returns([]);
        var list = _buyerBusinessLogicContract.GetAllBuyers();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllBuyers_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.GetAllBuyers(), Throws.TypeOf<NullListException>());
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllBuyers_StorageThrowError_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.GetList()).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _buyerBusinessLogicContract.GetAllBuyers(), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetById_ReturnRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var record = new BuyerDataModel(id, "fio", "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        var element = _buyerBusinessLogicContract.GetBuyerByData(id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetByFio_ReturnRecord_Test()
    {
        var fio = "fio";
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), fio, "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.GetElementByFIO(fio)).Returns(record);
        var element = _buyerBusinessLogicContract.GetBuyerByData(fio);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.FIO, Is.EqualTo(fio));
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_GetByPhoneNumber_ReturnRecord_Test()
    {
        var phoneNumber = "+7-111-111-11-11";
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", phoneNumber, 0);
        _buyerStorageContract.Setup(x => x.GetElementByPhoneNumber(phoneNumber)).Returns(record);
        var element = _buyerBusinessLogicContract.GetBuyerByData(phoneNumber);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.PhoneNumber, Is.EqualTo(phoneNumber));
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_EmptyData_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetByFio_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("fio"), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetBuyerByData_GetByPhoneNumber_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("+7-111-111-11-12"), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetBuyerByData_StorageThrowError_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _buyerStorageContract.Setup(x => x.GetElementByFIO(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _buyerStorageContract.Setup(x => x.GetElementByPhoneNumber(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("fio"), Throws.TypeOf<StorageException>());
        Assert.That(() => _buyerBusinessLogicContract.GetBuyerByData("+7-111-111-11-12"), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
        _buyerStorageContract.Verify(x => x.GetElementByPhoneNumber(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertBuyer_CorrectRecord_Test()
    {
        var flag = false;
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 10);
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>()))
            .Callback((BuyerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO && x.PhoneNumber == record.PhoneNumber && x.DiscountSize == record.DiscountSize;
            });
        _buyerBusinessLogicContract.InsertBuyer(record);
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertBuyer_RecordWithExistsData_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementExistsException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void InsertBuyer_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(null!), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertBuyer_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new BuyerDataModel("id", "fio", "+7-111-111-11-11", 10)), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertBuyer_StorageThrowError_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.AddElement(It.IsAny<BuyerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _buyerBusinessLogicContract.InsertBuyer(new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.AddElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_CorrectRecord_Test()
    {
        var flag = false;
        var record = new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0);
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>()))
            .Callback((BuyerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO && x.PhoneNumber == record.PhoneNumber && x.DiscountSize == record.DiscountSize;
            });
        _buyerBusinessLogicContract.UpdateBuyer(record);
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdateBuyer_RecordWithIncorrectData_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_RecordWithExistsData_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<ElementExistsException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateBuyer_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(null!), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateBuyer_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new BuyerDataModel("id", "fio", "+7-111-111-11-11", 10)), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateBuyer_StorageThrowError_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.UpdElement(It.IsAny<BuyerDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _buyerBusinessLogicContract.UpdateBuyer(new BuyerDataModel(Guid.NewGuid().ToString(), "fio", "+7-111-111-11-11", 0)), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.UpdElement(It.IsAny<BuyerDataModel>()), Times.Once);
    }

    [Test]
    public void DeleteBuyer_CorrectRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _buyerStorageContract.Setup(x => x.DelElement(It.Is<string>(s => s == id))).Callback(() => { flag = true; });
        _buyerBusinessLogicContract.DeleteBuyer(id);
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeleteBuyer_RecordWithIncorrectId_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeleteBuyer_IdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteBuyer_IdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer("id"), Throws.TypeOf<ValidationException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteBuyer_StorageThrowError_ThrowException_Test()
    {
        _buyerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _buyerBusinessLogicContract.DeleteBuyer(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _buyerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}
