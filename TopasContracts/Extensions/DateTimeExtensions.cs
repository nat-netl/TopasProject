namespace TopasContracts.Extensions;

public static class DateTimeExtensions
{
    public static bool IsDateNotOlder(this DateTime date, DateTime olderDate)
    {
        return date >= olderDate;
    }
}
