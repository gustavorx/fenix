using api.Shared;
using api.ValueObjects;

namespace api.Features.MonthlySummaries.GetMonthlySummary;

public sealed class GetMonthlySummaryRequestValidator : IValidator<GetMonthlySummaryRequest>
{
    public IReadOnlyList<AppError> Validate(GetMonthlySummaryRequest request)
    {
        var errors = new List<AppError>();

        if (request.Month is null)
        {
            errors.Add(AppError.Validation("monthly_summary.month.required", "Month is required."));
        }
        else if (!MonthPeriod.IsValidMonth(request.Month.Value))
        {
            errors.Add(AppError.Validation("monthly_summary.month.invalid", "Month must be between 1 and 12."));
        }

        if (request.Year is null)
        {
            errors.Add(AppError.Validation("monthly_summary.year.required", "Year is required."));
        }
        else if (!MonthPeriod.IsValidYear(request.Year.Value))
        {
            errors.Add(AppError.Validation("monthly_summary.year.invalid", "Year must be between 1 and 9999."));
        }

        return errors;
    }
}
