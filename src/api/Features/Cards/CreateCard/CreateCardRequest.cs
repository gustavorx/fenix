namespace api.Features.Cards.CreateCard;

public class CreateCardRequest
{
    public string? Name { get; set; }
    public decimal? Limit { get; set; }
    public int? ClosingDay { get; set; }
}
