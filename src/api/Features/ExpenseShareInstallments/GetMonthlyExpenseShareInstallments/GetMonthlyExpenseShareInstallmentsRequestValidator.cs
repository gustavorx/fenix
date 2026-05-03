using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;

public sealed class GetMonthlyExpenseShareInstallmentsRequestValidator :
    IValidator<GetMonthlyExpenseShareInstallmentsRequest>
{
    public IReadOnlyList<AppError> Validate(GetMonthlyExpenseShareInstallmentsRequest request)
    {
        var errors = new List<AppError>();

        if (!MonthPeriod.IsValidMonth(request.Month))
        {
            errors.Add(AppError.Validation("expense_share_installment.month.invalid", "Month must be between 1 and 12."));
        }

        if (!MonthPeriod.IsValidYear(request.Year))
        {
            errors.Add(AppError.Validation("expense_share_installment.year.invalid", "Year must be between 1 and 9999."));
        }

        return errors;
    }

}
