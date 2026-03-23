namespace api.Features.Expenses.GetMonthlyExpenses;

public class MonthlyExpensesResponse
{
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyCollection<MonthlyExpenseInstallmentResponse> Installments { get; init; } = [];
}

public class MonthlyExpenseInstallmentResponse
{
    public Guid InstallmentId { get; init; }
    public Guid ExpenseId { get; init; }
    public string Description { get; init; } = string.Empty;
    public string PaymentType { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public int TotalInstallments { get; init; }
    public int InstallmentNumber { get; init; }
    public decimal InstallmentAmount { get; init; }
    public DateTime PurchaseDate { get; init; }
    public DateTime DueDate { get; init; }
    public bool Paid { get; init; }
}
