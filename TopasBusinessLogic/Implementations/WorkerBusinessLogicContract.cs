using TopasContracts.BusinessLogicsContracts;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TopasBusinessLogic.Implementations;

internal class WorkerBusinessLogicContract(IWorkerStorageContract workerStorageContract, ILogger logger) : IWorkerBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IWorkerStorageContract _workerStorageContract = workerStorageContract;

    public List<WorkerDataModel> GetAllWorkers(bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {OnlyActive}", onlyActive);
        return _workerStorageContract.GetList(onlyActive) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByPost(string postId, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {PostId}, {OnlyActive}", postId, onlyActive);
        if (postId.IsEmpty())
            throw new ArgumentNullException(nameof(postId));
        if (!postId.IsGuid())
            throw new ValidationException("The value in the field postId is not a unique identifier.");
        return _workerStorageContract.GetList(onlyActive, postId) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByBirthDate(DateTime fromDate, DateTime toDate, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {OnlyActive}, {FromDate}, {ToDate}", onlyActive, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        return _workerStorageContract.GetList(onlyActive, fromBirthDate: fromDate, toBirthDate: toDate) ?? throw new NullListException();
    }

    public List<WorkerDataModel> GetAllWorkersByEmploymentDate(DateTime fromDate, DateTime toDate, bool onlyActive = true)
    {
        _logger.LogInformation("GetAllWorkers params: {OnlyActive}, {FromDate}, {ToDate}", onlyActive, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        return _workerStorageContract.GetList(onlyActive, fromEmploymentDate: fromDate, toEmploymentDate: toDate) ?? throw new NullListException();
    }

    public WorkerDataModel GetWorkerByData(string data)
    {
        _logger.LogInformation("Get element by data: {Data}", data);
        if (data.IsEmpty())
            throw new ArgumentNullException(nameof(data));
        if (data.IsGuid())
            return _workerStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        return _workerStorageContract.GetElementByFIO(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertWorker(WorkerDataModel workerDataModel)
    {
        _logger.LogInformation("New data: {Json}", JsonSerializer.Serialize(workerDataModel));
        ArgumentNullException.ThrowIfNull(workerDataModel);
        workerDataModel.Validate();
        _workerStorageContract.AddElement(workerDataModel);
    }

    public void UpdateWorker(WorkerDataModel workerDataModel)
    {
        _logger.LogInformation("Update data: {Json}", JsonSerializer.Serialize(workerDataModel));
        ArgumentNullException.ThrowIfNull(workerDataModel);
        workerDataModel.Validate();
        _workerStorageContract.UpdElement(workerDataModel);
    }

    public void DeleteWorker(string id)
    {
        _logger.LogInformation("Delete by id: {Id}", id);
        if (id.IsEmpty())
            throw new ArgumentNullException(nameof(id));
        if (!id.IsGuid())
            throw new ValidationException("Id is not a unique identifier");
        _workerStorageContract.DelElement(id);
    }
}
