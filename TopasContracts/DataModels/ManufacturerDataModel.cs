using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;

namespace TopasContracts.DataModels;

public class ManufacturerDataModel(string id, string manufacturerName, string? prevManufacturerName, string? prevPrevManufacturerName) : IValidation
{
    public string Id { get; private set; } = id;
    public string ManufacturerName { get; private set; } = manufacturerName;
    public string? PrevManufacturerName { get; private set; } = prevManufacturerName;
    public string? PrevPrevManufacturerName { get; private set; } = prevPrevManufacturerName;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");
        if (!Id.IsGuid())
            throw new ValidationException("The value in the field Id is not a unique identifier");
        if (ManufacturerName.IsEmpty())
            throw new ValidationException("Field ManufacturerName is empty");
    }
}
