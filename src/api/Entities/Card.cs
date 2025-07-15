namespace api.Entities;

public class Card
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal? Limit { get; set; }
    public int? ClosingDay { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public ICollection<Expense> Expenses { get; set; }
}
