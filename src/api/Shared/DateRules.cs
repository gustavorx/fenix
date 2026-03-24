namespace api.Shared;

public static class DateRules
{
    public static DateTime Normalize(this DateTime? date)
    {
        if (date == null)
        {
            return DateTime.UtcNow;
        }

        return date.Value.Kind switch
        {
            DateTimeKind.Utc => date.Value,
            DateTimeKind.Local => date.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
        };
    }
}