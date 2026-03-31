using api.ValueObjects;

namespace api.Entities;

public class Installment
{
    private Installment()
    {
        Expense = null!;
    }

    private Installment(Guid expenseId, int number, Money amount, DateOnly dueDate)
    {
        Id = Guid.NewGuid();
        ExpenseId = expenseId;
        Number = number;
        Amount = amount;
        DueDate = dueDate;
        Paid = false;
        Expense = null!;
    }

    public Guid Id { get; private set; }
    public int Number { get; private set; }
    public Money Amount { get; private set; }
    public DateOnly DueDate { get; private set; }
    public bool Paid { get; private set; }

    public Guid ExpenseId { get; private set; }
    public Expense Expense { get; private set; }

    internal static Installment Create(Guid expenseId, int number, Money amount, DateOnly dueDate)
    {
        return new Installment(expenseId, number, amount, dueDate);
    }
}