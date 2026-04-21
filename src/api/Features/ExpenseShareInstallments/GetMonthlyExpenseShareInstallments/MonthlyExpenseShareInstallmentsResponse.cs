namespace api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;

public class MonthlyExpenseShareInstallmentsResponse
{
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyCollection<MonthlyExpenseShareInstallmentResponse> Items { get; init; } = [];
}

public class MonthlyExpenseShareInstallmentResponse
{
    public Guid ShareInstallmentId { get; init; }
    public Guid ShareId { get; init; }
    public Guid ExpenseId { get; init; }
    public string ExpenseDescription { get; init; } = string.Empty;
    public Guid? PersonId { get; init; }
    public string? PersonName { get; init; }
    public decimal Amount { get; init; }
    public DateOnly DueDate { get; init; }
    public DateOnly? PaidDate { get; init; }
    public bool IsPaid { get; init; }
}
