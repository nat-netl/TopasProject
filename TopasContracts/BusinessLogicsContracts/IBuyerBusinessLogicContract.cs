using TopasContracts.DataModels;

namespace TopasContracts.BusinessLogicsContracts;

public interface IBuyerBusinessLogicContract
{
    List<BuyerDataModel> GetAllBuyers();
    BuyerDataModel GetBuyerByData(string data);
    void InsertBuyer(BuyerDataModel buyerDataModel);
    void UpdateBuyer(BuyerDataModel buyerDataModel);
    void DeleteBuyer(string id);
}
