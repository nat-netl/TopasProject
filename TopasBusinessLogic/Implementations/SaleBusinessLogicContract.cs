using TopasContracts.BusinessLogicsContracts;
using TopasContracts.DataModels;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.StoragesContracts;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace TopasBusinessLogic.Implementations;

internal class SaleBusinessLogicContract(ISaleStorageContract saleStorageContract, ILogger logger) : ISaleBusinessLogicContract
{
    private readonly ILogger _logger = logger;
    private readonly ISaleStorageContract _saleStorageContract = saleStorageContract;

    public List<SaleDataModel> GetAllSalesByPeriod(DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("GetAllSales params: {FromDate}, {ToDate}", fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        return _saleStorageContract.GetList(fromDate, toDate) ?? throw new NullListException();
    }

    public List<SaleDataModel> GetAllSalesByWorkerByPeriod(string workerId, DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("GetAllSales params: {WorkerId}, {FromDate}, {ToDate}", workerId, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        if (workerId.IsEmpty())
            throw new ArgumentNullException(nameof(workerId));
        if (!workerId.IsGuid())
            throw new ValidationException("The value in the field workerId is not a unique identifier.");
        return _saleStorageContract.GetList(fromDate, toDate, workerId: workerId) ?? throw new NullListException();
    }

    public List<SaleDataModel> GetAllSalesByBuyerByPeriod(string buyerId, DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("GetAllSales params: {BuyerId}, {FromDate}, {ToDate}", buyerId, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        if (buyerId.IsEmpty())
            throw new ArgumentNullException(nameof(buyerId));
        if (!buyerId.IsGuid())
            throw new ValidationException("The value in the field buyerId is not a unique identifier.");
        return _saleStorageContract.GetList(fromDate, toDate, buyerId: buyerId) ?? throw new NullListException();
    }

    public List<SaleDataModel> GetAllSalesByProductByPeriod(string productId, DateTime fromDate, DateTime toDate)
    {
        _logger.LogInformation("GetAllSales params: {ProductId}, {FromDate}, {ToDate}", productId, fromDate, toDate);
        if (fromDate.IsDateNotOlder(toDate))
            throw new IncorrectDatesException(fromDate, toDate);
        if (productId.IsEmpty())
            throw new ArgumentNullException(nameof(productId));
        if (!productId.IsGuid())
            throw new ValidationException("The value in the field productId is not a unique identifier.");
        return _saleStorageContract.GetList(fromDate, toDate, productId: productId) ?? throw new NullListException();
    }

    public SaleDataModel GetSaleByData(string data)
    {
        _logger.LogInformation("Get element by data: {Data}", data);
        if (data.IsEmpty())
            throw new ArgumentNullException(nameof(data));
        if (!data.IsGuid())
            throw new ValidationException("Id is not a unique identifier");
        return _saleStorageContract.GetElementById(data) ?? throw new ElementNotFoundException(data);
    }

    public void InsertSale(SaleDataModel saleDataModel)
    {
        _logger.LogInformation("New data: {Json}", JsonSerializer.Serialize(saleDataModel));
        ArgumentNullException.ThrowIfNull(saleDataModel);
        saleDataModel.Validate();
        _saleStorageContract.AddElement(saleDataModel);
    }

    public void CancelSale(string id)
    {
        _logger.LogInformation("Cancel by id: {Id}", id);
        if (id.IsEmpty())
            throw new ArgumentNullException(nameof(id));
        if (!id.IsGuid())
            throw new ValidationException("Id is not a unique identifier");
        _saleStorageContract.DelElement(id);
    }
}
