using TopasBusinessLogic.Implementations;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TopasTests.BusinessLogicsContractsTests;

[TestFixture]
internal class WorkerBusinessLogicContractTests
{
    private WorkerBusinessLogicContract _workerBusinessLogicContract = null!;
    private Mock<IWorkerStorageContract> _workerStorageContract = null!;

    private static WorkerDataModel CreateValidWorker(bool isDeleted = false) =>
        new(Guid.NewGuid().ToString(), "fio", Guid.NewGuid().ToString(), DateTime.Now.AddYears(-16).AddDays(-1), DateTime.Now, isDeleted);

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _workerStorageContract = new Mock<IWorkerStorageContract>();
        _workerBusinessLogicContract = new WorkerBusinessLogicContract(_workerStorageContract.Object, new Mock<ILogger>().Object);
    }

    [TearDown]
    public void TearDown()
    {
        _workerStorageContract.Reset();
    }

    [Test]
    public void GetAllWorkers_ReturnListOfRecords_Test()
    {
        var listOriginal = new List<WorkerDataModel> { CreateValidWorker(), CreateValidWorker(true), CreateValidWorker() };
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(listOriginal);
        var listOnlyActive = _workerBusinessLogicContract.GetAllWorkers(true);
        var list = _workerBusinessLogicContract.GetAllWorkers(false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _workerStorageContract.Verify(x => x.GetList(true, null, null, null, null, null), Times.Once);
        _workerStorageContract.Verify(x => x.GetList(false, null, null, null, null, null), Times.Once);
    }

    [Test]
    public void GetAllWorkers_ReturnEmptyList_Test()
    {
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns([]);
        var listOnlyActive = _workerBusinessLogicContract.GetAllWorkers(true);
        var list = _workerBusinessLogicContract.GetAllWorkers(false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Has.Count.EqualTo(0));
            Assert.That(list, Has.Count.EqualTo(0));
        });
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), null, null, null, null, null), Times.Exactly(2));
    }

    [Test]
    public void GetAllWorkers_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkers(true), Throws.TypeOf<NullListException>());
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Once);
    }

    [Test]
    public void GetAllWorkers_StorageThrowError_ThrowException_Test()
    {
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkers(true), Throws.TypeOf<StorageException>());
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), null, null, null, null, null), Times.Once);
    }

    [Test]
    public void GetAllWorkersByPost_ReturnListOfRecords_Test()
    {
        var postId = Guid.NewGuid().ToString();
        var listOriginal = new List<WorkerDataModel> { CreateValidWorker(), CreateValidWorker(true) };
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(listOriginal);
        var listOnlyActive = _workerBusinessLogicContract.GetAllWorkersByPost(postId, true);
        var list = _workerBusinessLogicContract.GetAllWorkersByPost(postId, false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _workerStorageContract.Verify(x => x.GetList(true, postId, null, null, null, null), Times.Once);
        _workerStorageContract.Verify(x => x.GetList(false, postId, null, null, null, null), Times.Once);
    }

    [Test]
    public void GetAllWorkersByPost_PostIdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkersByPost(null!, true), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkersByPost(string.Empty, true), Throws.TypeOf<ArgumentNullException>());
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Test]
    public void GetAllWorkersByPost_PostIdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkersByPost("postId", true), Throws.TypeOf<ValidationException>());
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Test]
    public void GetAllWorkersByBirthDate_ReturnListOfRecords_Test()
    {
        var date = DateTime.UtcNow;
        var listOriginal = new List<WorkerDataModel> { CreateValidWorker() };
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(listOriginal);
        var listOnlyActive = _workerBusinessLogicContract.GetAllWorkersByBirthDate(date, date.AddDays(1), true);
        var list = _workerBusinessLogicContract.GetAllWorkersByBirthDate(date, date.AddDays(1), false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _workerStorageContract.Verify(x => x.GetList(true, null, date, date.AddDays(1), null, null), Times.Once);
        _workerStorageContract.Verify(x => x.GetList(false, null, date, date.AddDays(1), null, null), Times.Once);
    }

    [Test]
    public void GetAllWorkersByBirthDate_IncorrectDates_ThrowException_Test()
    {
        var date = DateTime.UtcNow;
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkersByBirthDate(date, date, true), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _workerBusinessLogicContract.GetAllWorkersByBirthDate(date, date.AddSeconds(-1), true), Throws.TypeOf<IncorrectDatesException>());
        _workerStorageContract.Verify(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()), Times.Never);
    }

    [Test]
    public void GetAllWorkersByEmploymentDate_ReturnListOfRecords_Test()
    {
        var date = DateTime.UtcNow;
        var listOriginal = new List<WorkerDataModel> { CreateValidWorker() };
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(listOriginal);
        var listOnlyActive = _workerBusinessLogicContract.GetAllWorkersByEmploymentDate(date, date.AddDays(1), true);
        var list = _workerBusinessLogicContract.GetAllWorkersByEmploymentDate(date, date.AddDays(1), false);
        Assert.Multiple(() =>
        {
            Assert.That(listOnlyActive, Is.Not.Null);
            Assert.That(list, Is.Not.Null);
            Assert.That(listOnlyActive, Is.EquivalentTo(listOriginal));
            Assert.That(list, Is.EquivalentTo(listOriginal));
        });
        _workerStorageContract.Verify(x => x.GetList(true, null, null, null, date, date.AddDays(1)), Times.Once);
        _workerStorageContract.Verify(x => x.GetList(false, null, null, null, date, date.AddDays(1)), Times.Once);
    }

    [Test]
    public void GetWorkerByData_GetById_ReturnRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var record = new WorkerDataModel(id, "fio", Guid.NewGuid().ToString(), DateTime.Now.AddYears(-16).AddDays(-1), DateTime.Now, false);
        _workerStorageContract.Setup(x => x.GetElementById(id)).Returns(record);
        var element = _workerBusinessLogicContract.GetWorkerByData(id);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.Id, Is.EqualTo(id));
        _workerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetWorkerByData_GetByFio_ReturnRecord_Test()
    {
        var fio = "fio";
        var record = new WorkerDataModel(Guid.NewGuid().ToString(), fio, Guid.NewGuid().ToString(), DateTime.Now.AddYears(-16).AddDays(-1), DateTime.Now, false);
        _workerStorageContract.Setup(x => x.GetElementByFIO(fio)).Returns(record);
        var element = _workerBusinessLogicContract.GetWorkerByData(fio);
        Assert.That(element, Is.Not.Null);
        Assert.That(element.FIO, Is.EqualTo(fio));
        _workerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetWorkerByData_EmptyData_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetWorkerByData(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _workerBusinessLogicContract.GetWorkerByData(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _workerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _workerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetWorkerByData_GetById_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetWorkerByData(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _workerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Once);
        _workerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetWorkerByData_GetByFio_NotFoundRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.GetWorkerByData("fio"), Throws.TypeOf<ElementNotFoundException>());
        _workerStorageContract.Verify(x => x.GetElementById(It.IsAny<string>()), Times.Never);
        _workerStorageContract.Verify(x => x.GetElementByFIO(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void InsertWorker_CorrectRecord_Test()
    {
        var flag = false;
        var record = CreateValidWorker();
        _workerStorageContract.Setup(x => x.AddElement(It.IsAny<WorkerDataModel>()))
            .Callback((WorkerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO && x.PostId == record.PostId && x.BirthDate == record.BirthDate &&
                       x.EmploymentDate == record.EmploymentDate && x.IsDeleted == record.IsDeleted;
            });
        _workerBusinessLogicContract.InsertWorker(record);
        _workerStorageContract.Verify(x => x.AddElement(It.IsAny<WorkerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void InsertWorker_RecordWithExistsData_ThrowException_Test()
    {
        _workerStorageContract.Setup(x => x.AddElement(It.IsAny<WorkerDataModel>())).Throws(new ElementExistsException("Data", "Data"));
        Assert.That(() => _workerBusinessLogicContract.InsertWorker(CreateValidWorker()), Throws.TypeOf<ElementExistsException>());
        _workerStorageContract.Verify(x => x.AddElement(It.IsAny<WorkerDataModel>()), Times.Once);
    }

    [Test]
    public void InsertWorker_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.InsertWorker(null!), Throws.TypeOf<ArgumentNullException>());
        _workerStorageContract.Verify(x => x.AddElement(It.IsAny<WorkerDataModel>()), Times.Never);
    }

    [Test]
    public void InsertWorker_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.InsertWorker(new WorkerDataModel("id", "fio", Guid.NewGuid().ToString(), DateTime.Now.AddYears(-16).AddDays(-1), DateTime.Now, false)), Throws.TypeOf<ValidationException>());
        _workerStorageContract.Verify(x => x.AddElement(It.IsAny<WorkerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateWorker_CorrectRecord_Test()
    {
        var flag = false;
        var record = CreateValidWorker();
        _workerStorageContract.Setup(x => x.UpdElement(It.IsAny<WorkerDataModel>()))
            .Callback((WorkerDataModel x) =>
            {
                flag = x.Id == record.Id && x.FIO == record.FIO && x.PostId == record.PostId && x.BirthDate == record.BirthDate &&
                       x.EmploymentDate == record.EmploymentDate && x.IsDeleted == record.IsDeleted;
            });
        _workerBusinessLogicContract.UpdateWorker(record);
        _workerStorageContract.Verify(x => x.UpdElement(It.IsAny<WorkerDataModel>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void UpdateWorker_RecordWithIncorrectData_ThrowException_Test()
    {
        _workerStorageContract.Setup(x => x.UpdElement(It.IsAny<WorkerDataModel>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _workerBusinessLogicContract.UpdateWorker(CreateValidWorker()), Throws.TypeOf<ElementNotFoundException>());
        _workerStorageContract.Verify(x => x.UpdElement(It.IsAny<WorkerDataModel>()), Times.Once);
    }

    [Test]
    public void UpdateWorker_NullRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.UpdateWorker(null!), Throws.TypeOf<ArgumentNullException>());
        _workerStorageContract.Verify(x => x.UpdElement(It.IsAny<WorkerDataModel>()), Times.Never);
    }

    [Test]
    public void UpdateWorker_InvalidRecord_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.UpdateWorker(new WorkerDataModel("id", "fio", Guid.NewGuid().ToString(), DateTime.Now.AddYears(-16).AddDays(-1), DateTime.Now, false)), Throws.TypeOf<ValidationException>());
        _workerStorageContract.Verify(x => x.UpdElement(It.IsAny<WorkerDataModel>()), Times.Never);
    }

    [Test]
    public void DeleteWorker_CorrectRecord_Test()
    {
        var id = Guid.NewGuid().ToString();
        var flag = false;
        _workerStorageContract.Setup(x => x.DelElement(It.Is<string>(s => s == id))).Callback(() => { flag = true; });
        _workerBusinessLogicContract.DeleteWorker(id);
        _workerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
        Assert.That(flag);
    }

    [Test]
    public void DeleteWorker_RecordWithIncorrectId_ThrowException_Test()
    {
        _workerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new ElementNotFoundException(""));
        Assert.That(() => _workerBusinessLogicContract.DeleteWorker(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
        _workerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void DeleteWorker_IdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.DeleteWorker(null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _workerBusinessLogicContract.DeleteWorker(string.Empty), Throws.TypeOf<ArgumentNullException>());
        _workerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteWorker_IdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _workerBusinessLogicContract.DeleteWorker("id"), Throws.TypeOf<ValidationException>());
        _workerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void DeleteWorker_StorageThrowError_ThrowException_Test()
    {
        _workerStorageContract.Setup(x => x.DelElement(It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _workerBusinessLogicContract.DeleteWorker(Guid.NewGuid().ToString()), Throws.TypeOf<StorageException>());
        _workerStorageContract.Verify(x => x.DelElement(It.IsAny<string>()), Times.Once);
    }
}
