namespace TopasContracts.Enums;

[Flags]
public enum DiscountType
{
    None = 0,
    OnSale = 1,
    RegularCustomer = 2,
    Certificate = 4
}
