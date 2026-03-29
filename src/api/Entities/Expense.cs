using api.ValueObjects;

namespace api.Entities;

public class Expense
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public Money TotalAmount { get; set; }
    public DateOnly PurchaseDate { get; set; }
    public ExpensePaymentType PaymentType { get; set; }
    public int? InstallmentsQuantity { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? CardId { get; set; }
    public Card? Card { get; set; }

    public ICollection<Installment> Installments { get; set; } = [];
    public ICollection<ExpenseShare> Shares { get; set; } = [];
}
