namespace api.Features.Cards.Shared;

public class CardResponse
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal? Limit { get; init; }
    public int? ClosingDay { get; init; }
}
