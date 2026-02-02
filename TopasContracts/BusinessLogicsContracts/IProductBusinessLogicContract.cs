using TopasContracts.DataModels;

namespace TopasContracts.BusinessLogicsContracts;

public interface IProductBusinessLogicContract
{
    List<ProductDataModel> GetAllProducts(bool onlyActive = true);
    List<ProductDataModel> GetAllProductsByManufacturer(string manufacturerId, bool onlyActive = true);
    List<ProductHistoryDataModel> GetProductHistoryByProduct(string productId);
    ProductDataModel GetProductByData(string data);
    void InsertProduct(ProductDataModel productDataModel);
    void UpdateProduct(ProductDataModel productDataModel);
    void DeleteProduct(string id);
}
