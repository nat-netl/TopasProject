using TopasContracts.DataModels;

namespace TopasContracts.StoragesContracts;

public interface IPostStorageContract
{
    List<PostDataModel> GetList();
    List<PostDataModel> GetPostWithHistory(string postId);
    PostDataModel? GetElementById(string id);
    PostDataModel? GetElementByName(string name);
    void AddElement(PostDataModel postDataModel);
    void UpdElement(PostDataModel postDataModel);
    void DelElement(string id);
    void ResElement(string id);
}
