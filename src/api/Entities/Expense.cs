using api.ValueObjects;

namespace api.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public Money TotalAmount { get; set; }
    public DateTime Date { get; set; }
    public string Type { get; set; } // "Fixed" or "Variable"
    public int? InstallmentsQuantity { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    public Guid? CardId { get; set; }
    public Card Card { get; set; }

    public ICollection<Installment> Installments { get; set; }
    public ICollection<ExpenseShare> Shares { get; set; }
}
