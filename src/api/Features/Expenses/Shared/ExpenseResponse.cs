namespace api.Features.Expenses.Shared;

public class ExpenseResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime PurchaseDate { get; init; }
    public string PaymentType { get; init; } = string.Empty;
    public int TotalInstallments { get; init; }
    public IReadOnlyCollection<InstallmentResponse> Installments { get; init; } = [];
}

public class InstallmentResponse
{
    public Guid Id { get; init; }
    public int Number { get; init; }
    public decimal Amount { get; init; }
    public DateTime DueDate { get; init; }
    public bool Paid { get; init; }
}
