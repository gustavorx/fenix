using api.ValueObjects;

namespace api.Entities;

public class Card
{
    private Card()
    {
        Name = null!;
        User = null!;
    }

    private Card(string name, Money? limit, int? closingDay, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = NormalizeName(name);
        Limit = EnsurePositiveLimit(limit);
        ClosingDay = EnsureValidClosingDay(closingDay);
        UserId = userId;
        User = null!;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Money? Limit { get; private set; }
    public int? ClosingDay { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public ICollection<Expense> Expenses { get; private set; } = [];

    public static Card Create(string name, Money? limit, int? closingDay, Guid userId)
    {
        return new Card(name, limit, closingDay, userId);
    }

    public void Update(string name, Money? limit, int? closingDay)
    {
        Name = NormalizeName(name);
        Limit = EnsurePositiveLimit(limit);
        ClosingDay = EnsureValidClosingDay(closingDay);
    }

    private static string NormalizeName(string name)
    {
        var normalizedName = name.Trim();
        if (string.IsNullOrWhiteSpace(normalizedName))
        {
            throw new ArgumentException("Name is required.", nameof(name));
        }

        return normalizedName;
    }

    private static Money? EnsurePositiveLimit(Money? limit)
    {
        if (limit is not null && limit.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(limit), "Limit must be greater than zero.");
        }

        return limit;
    }

    private static int? EnsureValidClosingDay(int? closingDay)
    {
        if (closingDay is < 1 or > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(closingDay), "ClosingDay must be between 1 and 31.");
        }

        return closingDay;
    }
}
