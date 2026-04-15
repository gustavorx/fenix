using api.Auth;
using api.Data;
using api.Features.Cards.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Cards.UpdateCard;

public class UpdateCardUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<UpdateCardRequest> validator)
{
    public async Task<Result<CardResponse>> ExecuteAsync(
        Guid id,
        UpdateCardRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<CardResponse>.Failure(errors);
        }

        var card = await context.Cards
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (card == null)
        {
            return Result<CardResponse>.Failure(
                AppError.NotFound("card.not_found", "Card not found."));
        }

        card.Update(
            request.Name ?? card.Name,
            request.Limit.HasValue ? Money.Create(request.Limit.Value) : card.Limit,
            request.ClosingDay ?? card.ClosingDay);

        await context.SaveChangesAsync(cancellationToken);

        return Result<CardResponse>.Success(card.ToResponse());
    }
}
