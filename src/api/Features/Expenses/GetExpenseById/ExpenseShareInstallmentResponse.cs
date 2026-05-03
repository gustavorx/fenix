namespace api.Features.Expenses.GetExpenseById;

public class ExpenseShareInstallmentResponse
{
    public Guid Id { get; init; }
    public decimal Amount { get; init; }
    public DateOnly DueDate { get; init; }
    public DateOnly? PaidDate { get; init; }
    public bool IsPaid { get; init; }
}
