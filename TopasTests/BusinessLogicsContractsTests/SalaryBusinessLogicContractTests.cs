using TopasBusinessLogic.Implementations;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using Moq;

namespace TopasTests.BusinessLogicsContractsTests;

[TestFixture]
internal class SalaryBusinessLogicContractTests
{
    private SalaryBusinessLogicContract _salaryBusinessLogicContract = null!;
    private Mock<ISalaryStorageContract> _salaryStorageContract = null!;
    private Mock<ISaleStorageContract> _saleStorageContract = null!;
    private Mock<IPostStorageContract> _postStorageContract = null!;
    private Mock<IWorkerStorageContract> _workerStorageContract = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        _salaryStorageContract = new Mock<ISalaryStorageContract>();
        _saleStorageContract = new Mock<ISaleStorageContract>();
        _postStorageContract = new Mock<IPostStorageContract>();
        _workerStorageContract = new Mock<IWorkerStorageContract>();
        _salaryBusinessLogicContract = new SalaryBusinessLogicContract(
            _salaryStorageContract.Object,
            _saleStorageContract.Object,
            _postStorageContract.Object,
            _workerStorageContract.Object,
            new Mock<ILogger>().Object);
    }

    [TearDown]
    public void TearDown()
    {
        _salaryStorageContract.Reset();
        _saleStorageContract.Reset();
        _postStorageContract.Reset();
        _workerStorageContract.Reset();
    }

    [Test]
    public void GetAllSalariesByPeriod_ReturnListOfRecords_Test()
    {
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(1);
        var listOriginal = new List<SalaryDataModel>
        {
            new(Guid.NewGuid().ToString(), DateTime.UtcNow, 10),
            new(Guid.NewGuid().ToString(), DateTime.UtcNow.AddDays(1), 14),
        };
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(listOriginal);
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriod(startDate, endDate);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _salaryStorageContract.Verify(x => x.GetList(startDate, endDate, null), Times.Once);
    }

    [Test]
    public void GetAllSalariesByPeriod_ReturnEmptyList_Test()
    {
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns([]);
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1));
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(0));
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByPeriod_IncorrectDates_ThrowException_Test()
    {
        var dateTime = DateTime.UtcNow;
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(dateTime, dateTime), Throws.TypeOf<IncorrectDatesException>());
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(dateTime, dateTime.AddSeconds(-1)), Throws.TypeOf<IncorrectDatesException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalariesByPeriod_ReturnNull_ThrowException_Test()
    {
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<NullListException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByPeriod_StorageThrowError_ThrowException_Test()
    {
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Throws(new StorageException(new InvalidOperationException()));
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriod(DateTime.UtcNow, DateTime.UtcNow.AddDays(1)), Throws.TypeOf<StorageException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Once);
    }

    [Test]
    public void GetAllSalariesByPeriodByWorker_ReturnListOfRecords_Test()
    {
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(1);
        var workerId = Guid.NewGuid().ToString();
        var listOriginal = new List<SalaryDataModel> { new(workerId, DateTime.UtcNow, 10) };
        _salaryStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>())).Returns(listOriginal);
        var list = _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(startDate, endDate, workerId);
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.EquivalentTo(listOriginal));
        _salaryStorageContract.Verify(x => x.GetList(startDate, endDate, workerId), Times.Once);
    }

    [Test]
    public void GetAllSalariesByPeriodByWorker_WorkerIdIsNullOrEmpty_ThrowException_Test()
    {
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), null!), Throws.TypeOf<ArgumentNullException>());
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), string.Empty), Throws.TypeOf<ArgumentNullException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void GetAllSalariesByPeriodByWorker_WorkerIdIsNotGuid_ThrowException_Test()
    {
        Assert.That(() => _salaryBusinessLogicContract.GetAllSalariesByPeriodByWorker(DateTime.UtcNow, DateTime.UtcNow.AddDays(1), "workerId"), Throws.TypeOf<ValidationException>());
        _salaryStorageContract.Verify(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public void CalculateSalaryByMounth_CalculateSalary_Test()
    {
        var workerId = Guid.NewGuid().ToString();
        var saleSum = 1.2 * 5;
        var postSalary = 2000.0;
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, DiscountType.None, false, [new SaleProductDataModel(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), 5, 1.2)])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, postSalary));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false)]);
        var sum = 0.0;
        var expectedSum = postSalary + saleSum * 0.1;
        _salaryStorageContract.Setup(x => x.AddElement(It.IsAny<SalaryDataModel>())).Callback((SalaryDataModel x) => { sum = x.Salary; });
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        Assert.That(sum, Is.EqualTo(expectedSum));
    }

    [Test]
    public void CalculateSalaryByMounth_WithSeveralWorkers_Test()
    {
        var worker1Id = Guid.NewGuid().ToString();
        var worker2Id = Guid.NewGuid().ToString();
        var list = new List<WorkerDataModel>
        {
            new(worker1Id, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false),
            new(worker2Id, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false),
        };
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), worker1Id, null, DiscountType.None, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>())).Returns(list);
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        _salaryStorageContract.Verify(x => x.AddElement(It.IsAny<SalaryDataModel>()), Times.Exactly(list.Count));
    }

    [Test]
    public void CalculateSalaryByMounth_WithoutSalesByWorker_Test()
    {
        var postSalary = 2000.0;
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns([]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, postSalary));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false)]);
        var sum = 0.0;
        _salaryStorageContract.Setup(x => x.AddElement(It.IsAny<SalaryDataModel>())).Callback((SalaryDataModel x) => { sum = x.Salary; });
        _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow);
        Assert.That(sum, Is.EqualTo(postSalary));
    }

    [Test]
    public void CalculateSalaryByMounth_SaleStorageReturnNull_ThrowException_Test()
    {
        var workerId = Guid.NewGuid().ToString();
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000));
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false)]);
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }

    [Test]
    public void CalculateSalaryByMounth_PostStorageReturnNull_ThrowException_Test()
    {
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, DiscountType.None, false, [])]);
        _workerStorageContract.Setup(x => x.GetList(It.IsAny<bool>(), It.IsAny<string?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .Returns([new WorkerDataModel(workerId, "Test", Guid.NewGuid().ToString(), DateTime.UtcNow.AddYears(-20), DateTime.UtcNow, false)]);
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }

    [Test]
    public void CalculateSalaryByMounth_WorkerStorageReturnNull_ThrowException_Test()
    {
        var workerId = Guid.NewGuid().ToString();
        _saleStorageContract.Setup(x => x.GetList(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns([new SaleDataModel(Guid.NewGuid().ToString(), workerId, null, DiscountType.None, false, [])]);
        _postStorageContract.Setup(x => x.GetElementById(It.IsAny<string>())).Returns(new PostDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 2000));
        Assert.That(() => _salaryBusinessLogicContract.CalculateSalaryByMounth(DateTime.UtcNow), Throws.TypeOf<NullListException>());
    }
}
