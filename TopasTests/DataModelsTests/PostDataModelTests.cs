using TopasContracts.DataModels;
using TopasContracts.Enums;
using TopasContracts.Exceptions;

namespace TopasTests.DataModelsTests;

[TestFixture]
internal class PostDataModelTests
{
    [Test]
    public void IdIsNullOrEmptyTest()
    {
        var post = CreateDataModel(null, "name", PostType.Assistant, 10);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
        post = CreateDataModel(string.Empty, "name", PostType.Assistant, 10);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void IdIsNotGuidTest()
    {
        var post = CreateDataModel("id", "name", PostType.Assistant, 10);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PostNameIsEmptyTest()
    {
        var manufacturer = CreateDataModel(Guid.NewGuid().ToString(), null, PostType.Assistant, 10);
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
        manufacturer = CreateDataModel(Guid.NewGuid().ToString(), string.Empty, PostType.Assistant, 10);
        Assert.That(() => manufacturer.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void PostTypeIsNoneTest()
    {
        var post = CreateDataModel(Guid.NewGuid().ToString(), "name", PostType.None, 10);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void SalaryIsLessOrZeroTest()
    {
        var post = CreateDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, 0);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
        post = CreateDataModel(Guid.NewGuid().ToString(), "name", PostType.Assistant, -10);
        Assert.That(() => post.Validate(), Throws.TypeOf<ValidationException>());
    }

    [Test]
    public void AllFieldsIsCorrectTest()
    {
        var postId = Guid.NewGuid().ToString();
        var postName = "name";
        var postType = PostType.Assistant;
        var salary = 10;
        var post = CreateDataModel(postId, postName, postType, salary);
        Assert.That(() => post.Validate(), Throws.Nothing);
        Assert.Multiple(() =>
        {
            Assert.That(post.Id, Is.EqualTo(postId));
            Assert.That(post.PostName, Is.EqualTo(postName));
            Assert.That(post.PostType, Is.EqualTo(postType));
            Assert.That(post.Salary, Is.EqualTo(salary));
        });
    }

    private static PostDataModel CreateDataModel(string? id, string? postName, PostType postType, double salary) =>
        new(id, postName, postType, salary);
}
