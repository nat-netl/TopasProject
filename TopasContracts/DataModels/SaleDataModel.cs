using TopasContracts.Enums;
using TopasContracts.Exceptions;
using TopasContracts.Extensions;
using TopasContracts.Infrastructure;

namespace TopasContracts.DataModels;

public class SaleDataModel : IValidation
{
    public string Id { get; private set; }
    public string WorkerId { get; private set; }
    public string? BuyerId { get; private set; }
    public DateTime SaleDate { get; private set; } = DateTime.UtcNow;
    public double Sum { get; private set; }
    public DiscountType DiscountType { get; private set; }
    public double Discount { get; private set; }
    public bool IsCancel { get; private set; }
    public List<SaleProductDataModel>? Products { get; private set; }

    public SaleDataModel(string id, string workerId, string? buyerId, DiscountType discountType, bool isCancel, List<SaleProductDataModel>? products)
    {
        Id = id;
        WorkerId = workerId;
        BuyerId = buyerId;
        DiscountType = discountType;
        IsCancel = isCancel;
        Products = products ?? [];

        var percent = 0.0;
        foreach (DiscountType elem in Enum.GetValues<DiscountType>())
        {
            if ((elem & discountType) != 0)
            {
                switch (elem)
                {
                    case DiscountType.None:
                        break;
                    case DiscountType.OnSale:
                        percent += 0.1;
                        break;
                    case DiscountType.RegularCustomer:
                        percent += 0.5;
                        break;
                    case DiscountType.Certificate:
                        percent += 0.3;
                        break;
                }
            }
        }

        Sum = Products?.Sum(x => x.Price * x.Count) ?? 0;
        Discount = Sum * percent;
    }

    public void Validate()
    {
        if (Id.IsEmpty())
            throw new ValidationException("Field Id is empty");
        if (!Id.IsGuid())
            throw new ValidationException("The value in the field Id is not a unique identifier");
        if (WorkerId.IsEmpty())
            throw new ValidationException("Field WorkerId is empty");
        if (!WorkerId.IsGuid())
            throw new ValidationException("The value in the field WorkerId is not a unique identifier");
        if (!BuyerId?.IsGuid() ?? !BuyerId?.IsEmpty() ?? false)
            throw new ValidationException("The value in the field BuyerId is not a unique identifier");
        if (Sum <= 0)
            throw new ValidationException("Field Sum is less than or equal to 0");
        if ((Products?.Count ?? 0) == 0)
            throw new ValidationException("The sale must include products");
    }
}
