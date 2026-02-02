using TopasContracts.DataModels;

namespace TopasContracts.StoragesContracts;

public interface IBuyerStorageContract
{
    List<BuyerDataModel> GetList();
    BuyerDataModel? GetElementById(string id);
    BuyerDataModel? GetElementByPhoneNumber(string phoneNumber);
    BuyerDataModel? GetElementByFIO(string fio);
    void AddElement(BuyerDataModel buyerDataModel);
    void UpdElement(BuyerDataModel buyerDataModel);
    void DelElement(string id);
}
