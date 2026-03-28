namespace api.Features.Incomes.Shared;

public class IncomeResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateOnly ReceivedDate { get; init; }
}
