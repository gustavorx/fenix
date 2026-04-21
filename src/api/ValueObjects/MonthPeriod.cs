namespace api.ValueObjects;

public readonly record struct MonthPeriod
{
    private MonthPeriod(int month, int year)
    {
        Month = month;
        Year = year;
        StartDate = new DateOnly(year, month, 1);
        EndDate = new DateOnly(year, month, DateTime.DaysInMonth(year, month));
    }

    public int Month { get; }
    public int Year { get; }
    public DateOnly StartDate { get; }
    public DateOnly EndDate { get; }

    public static bool IsValidMonth(int month) => month is >= 1 and <= 12;

    public static bool IsValidYear(int year) => year is >= 1 and <= 9999;

    public static MonthPeriod Create(int month, int year)
    {
        if (!IsValidMonth(month))
        {
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12.");
        }

        if (!IsValidYear(year))
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be between 1 and 9999.");
        }

        return new MonthPeriod(month, year);
    }
}
