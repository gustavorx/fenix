using api.Shared;
using api.ValueObjects;

namespace api.Features.Incomes.GetMonthlyIncomes;

public sealed class GetMonthlyIncomesRequestValidator : IValidator<GetMonthlyIncomesRequest>
{
    public IReadOnlyList<AppError> Validate(GetMonthlyIncomesRequest request)
    {
        var errors = new List<AppError>();

        if (!MonthPeriod.IsValidMonth(request.Month))
        {
            errors.Add(AppError.Validation("income.month.invalid", "Month must be between 1 and 12."));
        }

        if (!MonthPeriod.IsValidYear(request.Year))
        {
            errors.Add(AppError.Validation("income.year.invalid", "Year must be between 1 and 9999."));
        }

        return errors;
    }

}
