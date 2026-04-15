using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Cards.DeleteCard;

public class DeleteCardUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var card = await context.Cards
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (card == null)
        {
            return Result.Failure(
                AppError.NotFound("card.not_found", "Card not found."));
        }

        context.Cards.Remove(card);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
