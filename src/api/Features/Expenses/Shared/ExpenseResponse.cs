using api.Entities;

namespace api.Features.Expenses.Shared;

public class ExpenseResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateOnly PurchaseDate { get; init; }
    public ExpensePaymentType PaymentType { get; init; }
    public int TotalInstallments { get; init; }
    public IReadOnlyCollection<InstallmentResponse> Installments { get; init; } = [];
}

public class InstallmentResponse
{
    public Guid Id { get; init; }
    public int Number { get; init; }
    public decimal Amount { get; init; }
    public DateOnly DueDate { get; init; }
    public bool Paid { get; init; }
}
