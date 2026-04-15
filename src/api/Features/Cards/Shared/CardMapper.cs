using api.Entities;

namespace api.Features.Cards.Shared;

public static class CardMapper
{
    public static CardResponse ToResponse(this Card card)
    {
        return new CardResponse
        {
            Id = card.Id,
            Name = card.Name,
            Limit = card.Limit?.Value,
            ClosingDay = card.ClosingDay
        };
    }
}
