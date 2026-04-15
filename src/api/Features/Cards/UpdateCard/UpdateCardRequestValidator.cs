using api.Shared;
using api.ValueObjects;

namespace api.Features.Cards.UpdateCard;

public sealed class UpdateCardRequestValidator : IValidator<UpdateCardRequest>
{
    public IReadOnlyList<AppError> Validate(UpdateCardRequest request)
    {
        var errors = new List<AppError>();

        if (request.Name is null && request.Limit is null && request.ClosingDay is null)
        {
            errors.Add(AppError.Validation("card.update.empty", "At least one field with a value must be provided."));
            return errors;
        }

        if (request.Name is not null)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                errors.Add(AppError.Validation("card.name.required", "Name is required."));
            }
            else if (request.Name.Trim().Length > 100)
            {
                errors.Add(AppError.Validation("card.name.length", "Name must have at most 100 characters."));
            }
        }

        if (request.Limit is <= 0)
        {
            errors.Add(AppError.Validation("card.limit.invalid", "Limit must be greater than zero."));
        }

        if (request.Limit is not null && !Money.HasValidScale(request.Limit.Value))
        {
            errors.Add(AppError.Validation("card.limit.scale", "Limit must have at most 2 decimal places."));
        }

        if (request.ClosingDay is < 1 or > 31)
        {
            errors.Add(AppError.Validation("card.closing_day.invalid", "ClosingDay must be between 1 and 31."));
        }

        return errors;
    }
}
