namespace api.ValueObjects;

public readonly record struct Money
{
    public decimal Value { get; }

    private Money(decimal value)
    {
        Value = value;
    }

    public static Money Zero => new(0m);

    public static Money Create(decimal value)
    {
        if (!HasValidScale(value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Money must have at most 2 decimal places.");
        }

        return new Money(value);
    }
    
    public static bool HasValidScale(decimal value) => decimal.Round(value, 2) == value;
    
    public static Money operator +(Money left, Money right) => Create(left.Value + right.Value);

    public static Money operator -(Money left, Money right) => Create(left.Value - right.Value);

    public static implicit operator decimal(Money money) => money.Value;
}
