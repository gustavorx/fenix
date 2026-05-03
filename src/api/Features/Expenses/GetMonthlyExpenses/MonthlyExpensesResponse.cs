using api.Entities;

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
    public Guid? CardId { get; init; }
    public string Description { get; init; } = string.Empty;
    public ExpensePaymentType PaymentType { get; init; }
    public decimal TotalAmount { get; init; }
    public int TotalInstallments { get; init; }
    public int InstallmentNumber { get; init; }
    public decimal InstallmentAmount { get; init; }
    public DateOnly PurchaseDate { get; init; }
    public DateOnly DueDate { get; init; }
    public bool Paid { get; init; }
    public bool HasShares { get; init; }
}
