namespace api.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public ICollection<Income> Incomes { get; set; } = [];
    public ICollection<Expense> Expenses { get; set; } = [];
    public ICollection<Person> People { get; set; } = [];
    public ICollection<Card> Cards { get; set; } = [];
}
