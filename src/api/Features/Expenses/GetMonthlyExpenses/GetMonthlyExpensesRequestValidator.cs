using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.GetMonthlyExpenses;

public sealed class GetMonthlyExpensesRequestValidator : IValidator<GetMonthlyExpensesRequest>
{
    public IReadOnlyList<AppError> Validate(GetMonthlyExpensesRequest request)
    {
        var errors = new List<AppError>();

        if (!MonthPeriod.IsValidMonth(request.Month))
        {
            errors.Add(AppError.Validation("expense.month.invalid", "Month must be between 1 and 12."));
        }

        if (!MonthPeriod.IsValidYear(request.Year))
        {
            errors.Add(AppError.Validation("expense.year.invalid", "Year must be between 1 and 9999."));
        }

        return errors;
    }

}
