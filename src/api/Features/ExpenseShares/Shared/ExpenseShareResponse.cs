namespace api.Features.ExpenseShares.Shared;

public class ExpenseShareResponse
{
    public Guid Id { get; init; }
    public Guid? PersonId { get; init; }
    public string? PersonName { get; init; }
    public decimal Amount { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal OutstandingAmount { get; init; }
    public bool IsFullyPaid { get; init; }
    public IReadOnlyCollection<ExpenseShareInstallmentResponse> Installments { get; init; } = [];
}
