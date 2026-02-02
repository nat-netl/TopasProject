using TopasBusinessLogic.Implementations;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TopasTests.BusinessLogicsContractsTests;

[TestFixture]
internal class ProductBusinessLogicContractTests
{
    private ProductBusinessLogicContract _productBusinessLogicContract = null!;
    private Mock<IProductStorageContract> _productStorageContract = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _productStorageContract = new Mock<IProductStorageContract>();
        _productBusinessLogicContract = new ProductBusinessLogicContract(_productStorageContract.Object, new Mock<ILogger>().Object);
    }

    [TearDown]
    public void TearDown()
    {
        _productStorageContract.Reset();
    }

    [Test]
    public void GetAllProducts_ReturnListOfRecords_Test()
    {
        var listOriginal = new List<ProductDataModel>
        {
            new(Guid.NewGuid().ToString(), "name 1", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false),
            new(Guid.NewGuid().ToString(), "name 2", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, true),
            new(Guid.NewGuid().ToString(), "name 3", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false),
        };
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Returns(listOriginal);
        var listOnlyActive = _productBusinessLogicContract.GetAllProducts(true);
        var list = _productBusinessLogicContract.GetAllProducts(false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _productStorageContract.Verify(x => x.GetList(true, null), Times.Once);
        _productStorageContract.Verify(x => x.GetList(false, null), Times.Once);
    }

    [Test]
    public void GetAllProducts_ReturnEmptyList_Test()
    {
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Returns([]);
        var listOnlyActive = _productBusinessLogicContract.GetAllProducts(true);
        var list = _productBusinessLogicContract.GetAllProducts(false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Has.Count.EqualTo(0));
            Assert.That(list, Has.Count.EqualTo(0));
        });
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), null), Times.Exactly(2));
    }

    [Test]
    public void GetAllProducts_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetAllProducts(true), Throws.TypeOf<NullListException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllProducts_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.GetAllProducts(true), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllProductsByManufacturer_ReturnListOfRecords_Test()
    {
        var manufacturerId = Guid.NewGuid().ToString();
        var listOriginal = new List<ProductDataModel>
        {
            new(Guid.NewGuid().ToString(), "name 1", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false),
            new(Guid.NewGuid().ToString(), "name 2", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, true),
            new(Guid.NewGuid().ToString(), "name 3", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false),
        };
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Returns(listOriginal);
        var listOnlyActive = _productBusinessLogicContract.GetAllProductsByManufacturer(manufacturerId, true);
        var list = _productBusinessLogicContract.GetAllProductsByManufacturer(manufacturerId, false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _productStorageContract.Verify(x => x.GetList(true, manufacturerId), Times.Once);
        _productStorageContract.Verify(x => x.GetList(false, manufacturerId), Times.Once);
    }

    [Test]
    public void GetAllProductsByManufacturer_ReturnEmptyList_Test()
    {
        var manufacturerId = Guid.NewGuid().ToString();
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Returns([]);
        var listOnlyActive = _productBusinessLogicContract.GetAllProductsByManufacturer(manufacturerId, true);
        var list = _productBusinessLogicContract.GetAllProductsByManufacturer(manufacturerId, false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Has.Count.EqualTo(0));
            Assert.That(list, Has.Count.EqualTo(0));
        });
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), manufacturerId), Times.Exactly(2));
    }

    [Test]
    public void GetAllProductsByManufacturer_ManufacturerIdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetAllProductsByManufacturer(null!, true), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _productBusinessLogicContract.GetAllProductsByManufacturer(string.Empty, true), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllProductsByManufacturer_ManufacturerIdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetAllProductsByManufacturer("manufacturerId", true), Throws.TypeOf<ValidationException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllProductsByManufacturer_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetAllProductsByManufacturer(Guid.NewGuid().ToString(), true), Throws.TypeOf<NullListException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllProductsByManufacturer_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.GetAllProductsByManufacturer(Guid.NewGuid().ToString(), true), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductHistoryByProduct_ReturnListOfRecords_Test()
    {
        var productId = Guid.NewGuid().ToString();
        var listOriginal = new List<ProductHistoryDataModel>
        {
            new(productId, 10),
            new(productId, 15),
            new(productId, 10),
        };
        _productStorageContract.Setup(x => x.GetHistoryByProductId(It.IsAny<string>())).Returns(listOriginal);
        var list = _productBusinessLogicContract.GetProductHistoryByProduct(productId);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _productStorageContract.Verify(x => x.GetHistoryByProductId(productId), Times.Once);
    }

    [Test]
    public void GetProductHistoryByProduct_ReturnEmptyList_Test()
    {
        _productStorageContract.Setup(x => x.GetHistoryByProductId(It.IsAny<string>())).Returns([]);
        var list = _productBusinessLogicContract.GetProductHistoryByProduct(Guid.NewGuid().ToString());
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _productStorageContract.Verify(x => x.GetHistoryByProductId(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductHistoryByProduct_ProductIdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductHistoryByProduct(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _productBusinessLogicContract.GetProductHistoryByProduct(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.GetHistoryByProductId(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetProductHistoryByProduct_ProductIdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductHistoryByProduct("productId"), Throws.TypeOf<ValidationException>());
        _productStorageContract.Verify(x => x.GetHistoryByProductId(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetProductHistoryByProduct_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductHistoryByProduct(Guid.NewGuid().ToString()), Throws.TypeOf<NullListException>());
        _productStorageContract.Verify(x => x.GetHistoryByProductId(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductHistoryByProduct_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.GetHistoryByProductId(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.GetProductHistoryByProduct(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.GetHistoryByProductId(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductByData_GetById_ReturnRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var record = new ProductDataModel(id, "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false);
        _productStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        var element = _productBusinessLogicContract.GetProductByData(id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _productStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductByData_GetByName_ReturnRecord_Test()
    {
        var name = "name";
        var record = new ProductDataModel(Guid.NewGuid().ToString(), name, ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false);
        _productStorageContract.Setup(x => x.GetElementByName(name)).Returns(record);
        var element = _productBusinessLogicContract.GetProductByData(name);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.ProductName, Is.EqualTo(name));
        _productStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductByData_EmptyData_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductByData(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _productBusinessLogicContract.GetProductByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _productStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetProductByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _productStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductByData_GetByName_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.GetProductByData("name"), Throws.TypeOf<ElementNotFoundException>());
        _productStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetProductByData_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _productStorageContract.Setup(x => x.GetElementByName(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.GetProductByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _productBusinessLogicContract.GetProductByData("name"), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _productStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertProduct_CorrectRecord_Test()
    {
        var flag = false;
        var record = new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false);
        _productStorageContract.Setup(x => x.AddElement(It.IsAny<ProductDataModel>()))
            .Callback((ProductDataModel x) =>
            {
                flag = x.Id == record.Id && x.ProductName == record.ProductName && x.ProductType == record.ProductType &&
                       x.ManufacturerId == record.ManufacturerId && x.Price == record.Price && x.IsDeleted == record.IsDeleted;
            });
        _productBusinessLogicContract.InsertProduct(record);
        _productStorageContract.Verify(x => x.AddElement(It.IsAny<ProductDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertProduct_RecordWithExistsData_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.AddElement(It.IsAny<ProductDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _productBusinessLogicContract.InsertProduct(new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<ElementExistsException>());
        _productStorageContract.Verify(x => x.AddElement(It.IsAny<ProductDataModel>()), Times.Once);
    }

    [Test]
    public void InsertProduct_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.InsertProduct(null!), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.AddElement(It.IsAny<ProductDataModel>()), Times.Never);
    }

    [Test]
    public void InsertProduct_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.InsertProduct(new ProductDataModel("id", "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<ValidationException>());
        _productStorageContract.Verify(x => x.AddElement(It.IsAny<ProductDataModel>()), Times.Never);
    }

    [Test]
    public void InsertProduct_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.AddElement(It.IsAny<ProductDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.InsertProduct(new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.AddElement(It.IsAny<ProductDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateProduct_CorrectRecord_Test()
    {
        var flag = false;
        var record = new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false);
        _productStorageContract.Setup(x => x.UpdElement(It.IsAny<ProductDataModel>()))
            .Callback((ProductDataModel x) =>
            {
                flag = x.Id == record.Id && x.ProductName == record.ProductName && x.ProductType == record.ProductType &&
                       x.ManufacturerId == record.ManufacturerId && x.Price == record.Price && x.IsDeleted == record.IsDeleted;
            });
        _productBusinessLogicContract.UpdateProduct(record);
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdateProduct_RecordWithIncorrectData_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.UpdElement(It.IsAny<ProductDataModel>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _productBusinessLogicContract.UpdateProduct(new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<ElementNotFoundException>());
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateProduct_RecordWithExistsData_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.UpdElement(It.IsAny<ProductDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _productBusinessLogicContract.UpdateProduct(new ProductDataModel(Guid.NewGuid().ToString(), "anme", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<ElementExistsException>());
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateProduct_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.UpdateProduct(null!), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateProduct_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.UpdateProduct(new ProductDataModel("id", "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<ValidationException>());
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateProduct_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.UpdElement(It.IsAny<ProductDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.UpdateProduct(new ProductDataModel(Guid.NewGuid().ToString(), "name", ProductType.Bracelet, Guid.NewGuid().ToString(), 10, false)), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.UpdElement(It.IsAny<ProductDataModel>()), Times.Once);
    }

    [Test]
    public void DeleteProduct_CorrectRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _productStorageContract.Setup(x => x.DelElement(It.Is<string>(s => s == id))).Callback(() => { flag = true; });
        _productBusinessLogicContract.DeleteProduct(id);
        _productStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeleteProduct_RecordWithIncorrectId_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _productBusinessLogicContract.DeleteProduct(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _productStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeleteProduct_IdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.DeleteProduct(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _productBusinessLogicContract.DeleteProduct(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _productStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteProduct_IdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _productBusinessLogicContract.DeleteProduct("id"), Throws.TypeOf<ValidationException>());
        _productStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteProduct_StorageThrowError_ThrowException_Test()
    {
        _productStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _productBusinessLogicContract.DeleteProduct(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _productStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}
