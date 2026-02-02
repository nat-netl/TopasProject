using TopasContracts.DataModels;

namespace TopasContracts.StoragesContracts;

public interface ISaleStorageContract
{
    List<SaleDataModel> GetList(DateTime? startDate = null, DateTime? endDate = null, string? workerId = null, string? buyerId = null, string? productId = null);
    SaleDataModel? GetElementById(string id);
    void AddElement(SaleDataModel saleDataModel);
    void DelElement(string id);
}
