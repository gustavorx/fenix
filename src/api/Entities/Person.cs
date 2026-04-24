namespace api.Entities;

public class Person
{
    private Person()
    {
        Name = null!;
        Phone = string.Empty;
        Email = string.Empty;
        User = null!;
    }

    private Person(string name, Guid userId)
    {
        Id = Guid.NewGuid();
        Name = NormalizeName(name);
        Phone = string.Empty;
        Email = string.Empty;
        UserId = userId;
        User = null!;
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Phone { get; private set; }
    public string Email { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public ICollection<ExpenseShare> Shares { get; private set; } = [];

    public static Person Create(string name, Guid userId)
    {
        return new Person(name, userId);
    }

    public void Update(string name)
    {
        Name = NormalizeName(name);
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
}
