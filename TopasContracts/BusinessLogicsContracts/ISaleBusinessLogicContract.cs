using TopasContracts.DataModels;

namespace TopasContracts.BusinessLogicsContracts;

public interface ISaleBusinessLogicContract
{
    List<SaleDataModel> GetAllSalesByPeriod(DateTime fromDate, DateTime toDate);
    List<SaleDataModel> GetAllSalesByWorkerByPeriod(string workerId, DateTime fromDate, DateTime toDate);
    List<SaleDataModel> GetAllSalesByBuyerByPeriod(string buyerId, DateTime fromDate, DateTime toDate);
    List<SaleDataModel> GetAllSalesByProductByPeriod(string productId, DateTime fromDate, DateTime toDate);
    SaleDataModel GetSaleByData(string data);
    void InsertSale(SaleDataModel saleDataModel);
    void CancelSale(string id);
}
