using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;

namespace TopasContracts.DataModels;

public class PostDataModel(string postId, string postName, PostType postType, double salary) : IValidation
{
    public string Id { get; private set; } = postId;
    public string PostName { get; private set; } = postName;
    public PostType PostType { get; private set; } = postType;
    public double Salary { get; private set; } = salary;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");
        if (!Id.IsGuid())
            throw new ValidationException("The value in the field Id is not a unique identifier");
        if (PostName.IsEmpty())
            throw new ValidationException("Field PostName is empty");
        if (PostType == PostType.None)
            throw new ValidationException("Field PostType is empty");
        if (Salary <= 0)
            throw new ValidationException("Field Salary is empty");
    }
}
