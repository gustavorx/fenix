using api.Auth;
using api.Data;
using api.Features.Cards.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Cards.GetAllCards;

public class GetAllCardsUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<CardResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await context.Cards
            .AsNoTracking()
            .Where(card => card.UserId == currentUser.UserId)
            .OrderBy(card => card.Name)
            .ThenBy(card => card.Id)
            .Select(card => new CardResponse
            {
                Id = card.Id,
                Name = card.Name,
                Limit = card.Limit == null ? null : card.Limit.Value,
                ClosingDay = card.ClosingDay
            })
            .ToListAsync(cancellationToken);
    }
}
