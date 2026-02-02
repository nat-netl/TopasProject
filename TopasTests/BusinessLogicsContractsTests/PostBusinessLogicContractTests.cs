using TopasBusinessLogic.Implementations;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TopasTests.BusinessLogicsContractsTests;

[TestFixture]
internal class PostBusinessLogicContractTests
{
    private PostBusinessLogicContract _postBusinessLogicContract = null!;
    private Mock<IPostStorageContract> _postStorageContract = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _postStorageContract = new Mock<IPostStorageContract>();
        _postBusinessLogicContract = new PostBusinessLogicContract(_postStorageContract.Object, new Mock<ILogger>().Object);
    }

    [TearDown]
    public void TearDown()
    {
        _postStorageContract.Reset();
    }

    [Test]
    public void GetAllPosts_ReturnListOfRecords_Test()
    {
        var listOriginal = new List<PostDataModel>
        {
            new(Guid.NewGuid().ToString(), "name 1", PostType.Assistant, 10),
            new(Guid.NewGuid().ToString(), "name 2", PostType.Assistant, 10),
            new(Guid.NewGuid().ToString(), "name 3", PostType.Assistant, 10),
        };
        _postStorageContract.Setup(x => x.GetList()).Returns(listOriginal);
        var list = _postBusinessLogicContract.GetAllPosts();
        Assert.Multiple(() =>
        {
            Assert.That(list, Is.Not.Null);
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _postStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllPosts_ReturnEmptyList_Test()
    {
        _postStorageContract.Setup(x => x.GetList()).Returns([]);
        var list = _postBusinessLogicContract.GetAllPosts();
        Assert.Multiple(() =>
        {
            Assert.That(list, Is.Not.Null);
            Assert.That(list, Has.Count.EqualTo(0));
        });
    }

    [Test]
    public void GetAllPosts_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetAllPosts(), Throws.TypeOf<NullListException>());
        _postStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllPosts_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.GetList()).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.GetAllPosts(), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetList(), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_ReturnListOfRecords_Test()
    {
        var postId = Guid.NewGuid().ToString();
        var listOriginal = new List<PostDataModel>
        {
            new(postId, "name 1", PostType.Assistant, 10),
            new(postId, "name 2", PostType.Assistant, 10)
        };
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Returns(listOriginal);
        var list = _postBusinessLogicContract.GetAllDataOfPost(postId);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(2));
        _postStorageContract.Verify(x => x.GetPostWithHistory(postId), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_ReturnEmptyList_Test()
    {
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Returns([]);
        var list = _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString());
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_PostIdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllDataOfPost_PostIdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllDataOfPost_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString()), Throws.TypeOf<NullListException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllDataOfPost_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.GetPostWithHistory(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.GetAllDataOfPost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetPostWithHistory(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_GetById_ReturnRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var record = new PostDataModel(id, "name", PostType.Assistant, 10);
        _postStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        var element = _postBusinessLogicContract.GetPostByData(id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_GetByName_ReturnRecord_Test()
    {
        var postName = "name";
        var record = new PostDataModel(Guid.NewGuid().ToString(), postName, PostType.Assistant, 10);
        _postStorageContract.Setup(x => x.GetElementByName(postName)).Returns(record);
        var element = _postBusinessLogicContract.GetPostByData(postName);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.PostName, Is.EqualTo(postName));
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_EmptyData_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetPostByData(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.GetPostByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetPostByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetPostByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetPostByData_GetByName_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.GetPostByData("name"), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetPostByData_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        _postStorageContract.Setup(x => x.GetElementByName(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.GetPostByData(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        Assert.That(() => _postBusinessLogicContract.GetPostByData("name"), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _postStorageContract.Verify(x => x.GetElementByName(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertPost_CorrectRecord_Test()
    {
        var flag = false;
        var record = new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10);
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>()))
            .Callback((PostDataModel x) => { flag = x.Id == record.Id && x.PostName == record.PostName && x.PostType == record.PostType && x.Salary == record.Salary; });
        _postBusinessLogicContract.InsertPost(record);
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertPost_RecordWithExistsData_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _postBusinessLogicContract.InsertPost(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10)), Throws.TypeOf<ElementExistsException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void InsertPost_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.InsertPost(null!), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void InsertPost_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.InsertPost(new PostDataModel("id", "name", PostType.Supervisor, 10)), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void InsertPost_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.AddElement(It.IsAny<PostDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.InsertPost(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10)), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.AddElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_CorrectRecord_Test()
    {
        var flag = false;
        var record = new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10);
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>()))
            .Callback((PostDataModel x) => { flag = x.Id == record.Id && x.PostName == record.PostName && x.PostType == record.PostType && x.Salary == record.Salary; });
        _postBusinessLogicContract.UpdatePost(record);
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdatePost_RecordWithIncorrectData_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10)), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_RecordWithExistsData_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new PostDataModel(Guid.NewGuid().ToString(), "anme", PostType.Supervisor, 10)), Throws.TypeOf<ElementExistsException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void UpdatePost_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.UpdatePost(null!), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void UpdatePost_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new PostDataModel("id", "name", PostType.Supervisor, 10)), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Never);
    }

    [Test]
    public void UpdatePost_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.UpdElement(It.IsAny<PostDataModel>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.UpdatePost(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Supervisor, 10)), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.UpdElement(It.IsAny<PostDataModel>()), Times.Once);
    }

    [Test]
    public void DeletePost_CorrectRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _postStorageContract.Setup(x => x.DelElement(It.Is<string>(s => s == id))).Callback(() => { flag = true; });
        _postBusinessLogicContract.DeletePost(id);
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeletePost_RecordWithIncorrectId_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _postBusinessLogicContract.DeletePost(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeletePost_IdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.DeletePost(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.DeletePost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeletePost_IdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.DeletePost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeletePost_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.DeletePost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void RestorePost_CorrectRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _postStorageContract.Setup(x => x.ResElement(It.Is<string>(s => s == id))).Callback(() => { flag = true; });
        _postBusinessLogicContract.RestorePost(id);
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void RestorePost_RecordWithIncorrectId_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.ResElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _postBusinessLogicContract.RestorePost(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void RestorePost_IdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.RestorePost(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _postBusinessLogicContract.RestorePost(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void RestorePost_IdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _postBusinessLogicContract.RestorePost("id"), Throws.TypeOf<ValidationException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void RestorePost_StorageThrowError_ThrowException_Test()
    {
        _postStorageContract.Setup(x => x.ResElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _postBusinessLogicContract.RestorePost(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _postStorageContract.Verify(x => x.ResElement(It.IsAny<string>()), Times.Once);
    }
}
