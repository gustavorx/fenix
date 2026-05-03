using api.Entities;
using api.Features.ExpenseShares.Shared;
using api.Features.Expenses.Shared;

namespace api.Features.Expenses.GetExpenseById;

public class ExpenseDetailResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateOnly PurchaseDate { get; init; }
    public ExpensePaymentType PaymentType { get; init; }
    public int TotalInstallments { get; init; }
    public Guid? CardId { get; init; }
    public IReadOnlyCollection<InstallmentResponse> Installments { get; init; } = [];
    public IReadOnlyCollection<ExpenseShareResponse> Shares { get; init; } = [];
}
