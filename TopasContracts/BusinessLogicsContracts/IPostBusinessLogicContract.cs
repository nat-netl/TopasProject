using TopasContracts.DataModels;

namespace TopasContracts.BusinessLogicsContracts;

public interface IPostBusinessLogicContract
{
    List<PostDataModel> GetAllPosts();
    List<PostDataModel> GetAllDataOfPost(string postId);
    PostDataModel GetPostByData(string data);
    void InsertPost(PostDataModel postDataModel);
    void UpdatePost(PostDataModel postDataModel);
    void DeletePost(string id);
    void RestorePost(string id);
}
