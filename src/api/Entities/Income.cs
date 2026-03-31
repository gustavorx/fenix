using api.ValueObjects;

namespace api.Entities;

public class Income
{
    private Income()
    {
        Description = null!;
        User = null!;
    }

    private Income(string description, Money amount, DateOnly receivedDate, Guid userId)
    {
        Id = Guid.NewGuid();
        Description = description;
        Amount = amount;
        ReceivedDate = receivedDate;
        UserId = userId;
        User = null!;
    }

    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public Money Amount { get; private set; }
    public DateOnly ReceivedDate { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public static Income Create(string description, Money amount, DateOnly receivedDate, Guid userId)
    {
        var normalizedDescription = description.Trim();
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (amount.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        return new Income(normalizedDescription!, amount, receivedDate, userId);
    }
}
