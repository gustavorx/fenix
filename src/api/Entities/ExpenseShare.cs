using api.ValueObjects;

namespace api.Entities;

public class ExpenseShare
{
    public Guid Id { get; set; }
    public Money Amount { get; set; }
    public bool Paid { get; set; }
    public DateTime? PaymentDate { get; set; }

    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; } = null!;

    public Guid? PersonId { get; set; }
    public Person? Person { get; set; }
}
