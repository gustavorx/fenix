using api.Shared;
using api.ValueObjects;

namespace api.Features.Incomes.UpdateIncome;

public sealed class UpdateIncomeRequestValidator : IValidator<UpdateIncomeRequest>
{
    public IReadOnlyList<AppError> Validate(UpdateIncomeRequest request)
    {
        var errors = new List<AppError>();

        if (request.Description is null && request.Amount is null && request.ReceivedDate is null)
        {
            errors.Add(AppError.Validation("income.update.empty", "At least one field must be provided."));
            return errors;
        }

        if (request.Description is not null && string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("income.description.required", "Description is required."));
        }

        if (request.Amount is <= 0)
        {
            errors.Add(AppError.Validation("income.amount.invalid", "Amount must be greater than zero."));
        }

        if (request.Amount is not null && !Money.HasValidScale(request.Amount.Value))
        {
            errors.Add(AppError.Validation("income.amount.scale", "Amount must have at most 2 decimal places."));
        }

        return errors;
    }
}
