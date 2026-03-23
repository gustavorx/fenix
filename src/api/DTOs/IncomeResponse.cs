namespace api.DTOs;

public class IncomeResponse
{
    public Guid Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime Date { get; init; }
}
