using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;
using System.Text.RegularExpressions;

namespace TopasContracts.DataModels;

public class BuyerDataModel(string id, string fio, string phoneNumber, double discountSize) : IValidation
{
    public string Id { get; private set; } = id;
    public string FIO { get; private set; } = fio;
    public string PhoneNumber { get; private set; } = phoneNumber;
    public double DiscountSize { get; private set; } = discountSize;

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");
        if (!Id.IsGuid())
            throw new ValidationException("The value in the field Id is not a unique identifier");
        if (FIO.IsEmpty())
            throw new ValidationException("Field FIO is empty");
        if (PhoneNumber.IsEmpty())
            throw new ValidationException("Field PhoneNumber is empty");
        if (!Regex.IsMatch(PhoneNumber, @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$"))
            throw new ValidationException("Field PhoneNumber is not a phone number");
    }
}
