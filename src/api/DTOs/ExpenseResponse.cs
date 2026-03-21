namespace api.DTOs;

public class ExpenseResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public DateTime Date { get; init; }
    public string PaymentType { get; init; } = string.Empty;
    public int? InstallmentsQuantity { get; init; }
    public int? CurrentInstallmentNumber { get; init; }
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
