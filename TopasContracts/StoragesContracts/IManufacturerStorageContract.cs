using TopasContracts.DataModels;

namespace TopasContracts.StoragesContracts;

public interface IManufacturerStorageContract
{
    List<ManufacturerDataModel> GetList();
    ManufacturerDataModel? GetElementById(string id);
    ManufacturerDataModel? GetElementByName(string name);
    ManufacturerDataModel? GetElementByOldName(string name);
    void AddElement(ManufacturerDataModel manufacturerDataModel);
    void UpdElement(ManufacturerDataModel manufacturerDataModel);
    void DelElement(string id);
}
