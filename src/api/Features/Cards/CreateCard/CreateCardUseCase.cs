using api.Auth;
using api.Data;
using api.Entities;
using api.Features.Cards.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Cards.CreateCard;

public class CreateCardUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<CreateCardRequest> validator)
{
    public async Task<Result<CardResponse>> ExecuteAsync(CreateCardRequest request, CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<CardResponse>.Failure(errors);
        }

        var card = Card.Create(
            request.Name!,
            request.Limit.HasValue ? Money.Create(request.Limit.Value) : null,
            request.ClosingDay,
            currentUser.UserId);

        context.Cards.Add(card);
        await context.SaveChangesAsync(cancellationToken);

        return Result<CardResponse>.Success(card.ToResponse());
    }
}
