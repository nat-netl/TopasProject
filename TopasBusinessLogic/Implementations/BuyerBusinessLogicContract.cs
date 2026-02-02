using TopasContracts.BusinessLogicsContracts;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace TopasBusinessLogic.Implementations;

internal class BuyerBusinessLogicContract(IBuyerStorageContract buyerStorageContract, ILogger logger) : IBuyerBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly IBuyerStorageContract _buyerStorageContract = buyerStorageContract;

    public List<BuyerDataModel> GetAllBuyers()
    {
        _logger.LogInformation("GetAllBuyers");
        return _buyerStorageContract.GetList() ?? throw new NullListException();
    }

    public BuyerDataModel GetBuyerByData(string data)
    {
        _logger.LogInformation("Get element by data: {Data}", data);
        if (data.IsEmpty())
            throw new ArgumentNullException(nameof(data));
        if (data.IsGuid())
            return _buyerStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
        if (Regex.IsMatch(data, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
            return _buyerStorageContract.GetElementByPhoneNumber(data) ?? throw new ElementNotFoundException(data);
        return _buyerStorageContract.GetElementByFIO(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertBuyer(BuyerDataModel buyerDataModel)
    {
        _logger.LogInformation("New data: {Json}", JsonSerializer.Serialize(buyerDataModel));
        ArgumentNullException.ThrowIfNull(buyerDataModel);
        buyerDataModel.Validate();
        _buyerStorageContract.AddElement(buyerDataModel);
    }

    public void UpdateBuyer(BuyerDataModel buyerDataModel)
    {
        _logger.LogInformation("Update data: {Json}", JsonSerializer.Serialize(buyerDataModel));
        ArgumentNullException.ThrowIfNull(buyerDataModel);
        buyerDataModel.Validate();
        _buyerStorageContract.UpdElement(buyerDataModel);
    }

    public void DeleteBuyer(string id)
    {
        _logger.LogInformation("Delete by id: {Id}", id);
        if (id.IsEmpty())
            throw new ArgumentNullException(nameof(id));
        if (!id.IsGuid())
            throw new ValidationException("Id is not a unique identifier");
        _buyerStorageContract.DelElement(id);
    }
}
