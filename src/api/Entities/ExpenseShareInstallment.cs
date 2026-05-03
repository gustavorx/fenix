using api.ValueObjects;

namespace api.Entities;

public class ExpenseShareInstallment
{
    private ExpenseShareInstallment()
    {
        ExpenseShare = null!;
    }

    private ExpenseShareInstallment(
        Guid expenseShareId,
        Money amount,
        DateOnly dueDate,
        DateOnly? paidDate)
    {
        if (amount.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        }

        Id = Guid.NewGuid();
        ExpenseShareId = expenseShareId;
        Amount = amount;
        DueDate = dueDate;
        PaidDate = paidDate;
        ExpenseShare = null!;
    }

    public Guid Id { get; private set; }
    public Money Amount { get; private set; }
    public DateOnly DueDate { get; private set; }
    public DateOnly? PaidDate { get; private set; }
    public bool IsPaid => PaidDate != null;

    public Guid ExpenseShareId { get; private set; }
    public ExpenseShare ExpenseShare { get; private set; }

    internal static ExpenseShareInstallment Create(
        Guid expenseShareId,
        Money amount,
        DateOnly dueDate,
        DateOnly? paidDate = null)
    {
        return new ExpenseShareInstallment(expenseShareId, amount, dueDate, paidDate);
    }
}
