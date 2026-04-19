namespace api.Features.Expenses.Shared;

public class InstallmentResponse
{
    public Guid Id { get; init; }
    public int Number { get; init; }
    public decimal Amount { get; init; }
    public DateOnly DueDate { get; init; }
    public bool Paid { get; init; }
}
