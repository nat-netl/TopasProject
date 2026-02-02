using TopasContracts.BusinessLogicsContracts;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;

namespace TopasBusinessLogic.Implementations;

internal class SalaryBusinessLogicContract(
    ISalaryStorageContract salaryStorageContract,
    ISaleStorageContract saleStorageContract,
    IPostStorageContract postStorageContract,
    IWorkerStorageContract workerStorageContract,
    ILogger logger) : ISalaryBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly ISalaryStorageContract _salaryStorageContract = salaryStorageContract;
    private readonly ISaleStorageContract _saleStorageContract = saleStorageContract;
    private readonly IPostStorageContract _postStorageContract = postStorageContract;
    private readonly IWorkerStorageContract _workerStorageContract = workerStorageContract;

    public List<SalaryDataModel> GetAllSalariesByPeriod(DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("GetAllSalaries params: {FromDate}, {ToDate}", fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        return _salaryStorageContract.GetList(fromDate, toDate) ?? throw new NullListException();
    }

    public List<SalaryDataModel> GetAllSalariesByPeriodByWorker(DateTime fromDate, DateTime toDate, string workerId)
    {
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        if (workerId.IsEmpty())
            throw new ArgumentNullException(nameof(workerId));
        if (!workerId.IsGuid())
            throw new ValidationException("The value in the field workerId is not a unique identifier.");
        _logger.LogInformation("GetAllSalaries params: {FromDate}, {ToDate}, {WorkerId}", fromDate, toDate, workerId);
        return _salaryStorageContract.GetList(fromDate, toDate, workerId) ?? throw new NullListException();
    }

    public void CalculateSalaryByMounth(DateTime date)
    {
        _logger.LogInformation("CalculateSalaryByMounth: {Date}", date);
        var startDate = new DateTime(date.Year, date.Month, 1);
        var finishDate = new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        var workers = _workerStorageContract.GetList() ?? throw new NullListException();
        foreach (var worker in workers)
        {
            var sales = _saleStorageContract.GetList(startDate, finishDate, workerId: worker.Id)?.Sum(x => x.Sum) ?? throw new NullListException();
            var post = _postStorageContract.GetElementById(worker.PostId) ?? throw new NullListException();
            var salary = post.Salary + sales * 0.1;
            _logger.LogDebug("The employee {WorkerId} was paid a salary of {Salary}", worker.Id, salary);
            _salaryStorageContract.AddElement(new SalaryDataModel(worker.Id, finishDate, salary));
        }
    }
}
