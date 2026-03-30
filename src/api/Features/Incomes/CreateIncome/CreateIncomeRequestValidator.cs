using api.Shared;
using api.ValueObjects;

namespace api.Features.Incomes.CreateIncome;

public sealed class CreateIncomeRequestValidator : IValidator<CreateIncomeRequest>
{
    public IReadOnlyList<AppError> Validate(CreateIncomeRequest request)
    {
        var errors = new List<AppError>();

        ValidateDescription(request, errors);
        ValidateAmount(request, errors);

        return errors;
    }

    private static void ValidateDescription(CreateIncomeRequest request, List<AppError> errors)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("income.description.required", "Description is required."));
        }
    }

    private static void ValidateAmount(CreateIncomeRequest request, List<AppError> errors)
    {
        if (request.Amount <= 0)
        {
            errors.Add(AppError.Validation("income.amount.invalid", "Amount must be greater than zero."));
        }

        if (!Money.HasValidScale(request.Amount))
        {
            errors.Add(AppError.Validation("income.amount.scale", "Amount must have at most 2 decimal places."));
        }
    }
}
