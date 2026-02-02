using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;

namespace TopasContracts.DataModels;

public class SaleProductDataModel(string saleId, string productId, int count, double price) : IValidation
{
    public string SaleId { get; private set; } = saleId;
    public string ProductId { get; private set; } = productId;
    public int Count { get; private set; } = count;
    public double Price { get; private set; } = price;

    public void Validate()
    {
        if (SaleId.IsEmpty())
            throw new ValidationException("Field SaleId is empty");
        if (!SaleId.IsGuid())
            throw new ValidationException("The value in the field SaleId is not a unique identifier");
        if (ProductId.IsEmpty())
            throw new ValidationException("Field ProductId is empty");
        if (!ProductId.IsGuid())
            throw new ValidationException("The value in the field ProductId is not a unique identifier");
        if (Count <= 0)
            throw new ValidationException("Field Count is less than or equal to 0");
        if (Price <= 0)
            throw new ValidationException("Field Price is less than or equal to 0");
    }
}
