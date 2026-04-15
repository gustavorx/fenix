using api.Auth;
using api.Data;
using api.Features.Cards.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Cards.GetCardById;

public class GetCardByIdUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<CardResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await context.Cards
            .AsNoTracking()
            .Where(item => item.Id == id && item.UserId == currentUser.UserId)
            .Select(card => new CardResponse
            {
                Id = card.Id,
                Name = card.Name,
                Limit = card.Limit == null ? null : card.Limit.Value,
                ClosingDay = card.ClosingDay
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (card == null)
        {
            return Result<CardResponse>.Failure(
                AppError.NotFound("card.not_found", "Card not found."));
        }

        return Result<CardResponse>.Success(card);
    }
}
