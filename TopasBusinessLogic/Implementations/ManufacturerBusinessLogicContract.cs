using TopasContracts.BusinessLogicsContracts;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TopasBusinessLogic.Implementations;

internal class ManufacturerBusinessLogicContract(IManufacturerStorageContract manufacturerStorageContract, ILogger logger) : IManufacturerBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IManufacturerStorageContract _manufacturerStorageContract = manufacturerStorageContract;

    public List<ManufacturerDataModel> GetAllManufacturers()
    {
        _logger.LogInformation("GetAllManufacturers");
        return _manufacturerStorageContract.GetList() ?? throw new NullListException();
    }

    public ManufacturerDataModel GetManufacturerByData(string data)
    {
        _logger.LogInformation("Get element by data: {Data}", data);
        if (data.IsEmpty())
            throw new ArgumentNullException(nameof(data));
        if (data.IsGuid())
            return _manufacturerStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        return _manufacturerStorageContract.GetElementByName(data) ?? _manufacturerStorageContract.GetElementByOldName(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertManufacturer(ManufacturerDataModel manufacturerDataModel)
    {
        _logger.LogInformation("New data: {Json}", JsonSerializer.Serialize(manufacturerDataModel));
        ArgumentNullException.ThrowIfNull(manufacturerDataModel);
        manufacturerDataModel.Validate();
        _manufacturerStorageContract.AddElement(manufacturerDataModel);
    }

    public void UpdateManufacturer(ManufacturerDataModel manufacturerDataModel)
    {
        _logger.LogInformation("Update data: {Json}", JsonSerializer.Serialize(manufacturerDataModel));
        ArgumentNullException.ThrowIfNull(manufacturerDataModel);
        manufacturerDataModel.Validate();
        _manufacturerStorageContract.UpdElement(manufacturerDataModel);
    }

    public void DeleteManufacturer(string id)
    {
        _logger.LogInformation("Delete by id: {Id}", id);
        if (id.IsEmpty())
            throw new ArgumentNullException(nameof(id));
        if (!id.IsGuid())
            throw new ValidationException("Id is not a unique identifier");
        _manufacturerStorageContract.DelElement(id);
    }
}
