using Microsoft.EntityFrameworkCore;
using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasDatabase.Implementations;
using TopasDatabase.Models;

namespace TopasTests.StoragesContracts;

[TestFixture]
internal class PostStorageContractTests : BaseStorageContractTest
{
    private PostStorageContract _postStorageContract = null!;

    [SetUp]
    public void SetUp()
    {
        _postStorageContract = new PostStorageContract(TopasDbContext);
    }

    [TearDown]
    public void TearDown()
    {
        TopasDbContext.Database.ExecuteSqlRaw("TRUNCATE \"Posts\" CASCADE;");
    }

    [Test]
    public void Try_GetList_WhenHaveRecords_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 1");
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 2");
        InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), "name 3");
        var list = _postStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Has.Count.EqualTo(3));
        AssertElement(list.First(x => x.Id == post.PostId), post);
    }

    [Test]
    public void Try_GetList_WhenNoRecords_Test()
    {
        var list = _postStorageContract.GetList();
        Assert.That(list, Is.Not.Null);
        Assert.That(list, Is.Empty);
    }

    [Test]
    public void Try_GetElementById_WhenHaveRecord_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString());
        AssertElement(_postStorageContract.GetElementById(post.PostId), post);
    }

    [Test]
    public void Try_AddElement_Test()
    {
        var post = CreateModel(Guid.NewGuid().ToString());
        _postStorageContract.AddElement(post);
        AssertElement(GetPostFromDatabaseByPostId(post.Id), post);
    }

    [Test]
    public void Try_DelElement_Test()
    {
        var post = InsertPostToDatabaseAndReturn(Guid.NewGuid().ToString(), isActual: true);
        _postStorageContract.DelElement(post.PostId);
        var element = GetPostFromDatabaseByPostId(post.PostId);
        Assert.That(element, Is.Not.Null);
        Assert.That(element!.IsActual, Is.False);
    }

    [Test]
    public void Try_DelElement_WhenNoRecordWithThisId_Test()
    {
        Assert.That(() => _postStorageContract.DelElement(Guid.NewGuid().ToString()), Throws.TypeOf<ElementNotFoundException>());
    }

    private Post InsertPostToDatabaseAndReturn(string id, string postName = "test", PostType postType = PostType.Assistant, double salary = 10, bool isActual = true, DateTime? changeDate = null)
    {
        var post = new Post { Id = Guid.NewGuid().ToString(), PostId = id, PostName = postName, PostType = postType, Salary = salary, IsActual = isActual, ChangeDate = changeDate ?? DateTime.UtcNow };
        TopasDbContext.Posts.Add(post);
        TopasDbContext.SaveChanges();
        return post;
    }

    private static void AssertElement(PostDataModel? actual, Post expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.Id, Is.EqualTo(expected.PostId));
        Assert.That(actual.PostName, Is.EqualTo(expected.PostName));
    }

    private static PostDataModel CreateModel(string postId, string postName = "test", PostType postType = PostType.Assistant, double salary = 10)
        => new(postId, postName, postType, salary);

    private Post? GetPostFromDatabaseByPostId(string id) => TopasDbContext.Posts.Where(x => x.PostId == id).OrderByDescending(x => x.ChangeDate).FirstOrDefault();

    private static void AssertElement(Post? actual, PostDataModel expected)
    {
        Assert.That(actual, Is.Not.Null);
        Assert.That(actual!.PostId, Is.EqualTo(expected.Id));
        Assert.That(actual.PostName, Is.EqualTo(expected.PostName));
    }
}
