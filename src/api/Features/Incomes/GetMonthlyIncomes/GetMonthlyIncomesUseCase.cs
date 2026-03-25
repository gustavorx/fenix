using api.Data;
using api.Features.Incomes.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.GetMonthlyIncomes;

public class GetMonthlyIncomesUseCase(FenixContext context)
{
    public async Task<Result<MonthlyIncomesResponse>> ExecuteAsync(int month, int year, CancellationToken cancellationToken)
    {
        if (month is < 1 or > 12)
        {
            return Result<MonthlyIncomesResponse>.Failure(
                AppError.Validation("income.month.invalid", "Month must be between 1 and 12."));
        }

        if (year is < 1 or > 9999)
        {
            return Result<MonthlyIncomesResponse>.Failure(
                AppError.Validation("income.year.invalid", "Year must be between 1 and 9999."));
        }

        var incomes = await context.Incomes
            .AsNoTracking()
            .Where(income => income.Date.Month == month && income.Date.Year == year)
            .OrderByDescending(income => income.Date)
            .ToListAsync(cancellationToken);

        return Result<MonthlyIncomesResponse>.Success(
            new MonthlyIncomesResponse
            {
                Month = month,
                Year = year,
                TotalAmount = incomes.Aggregate(Money.Zero, (total, income) => total + income.Amount).Value,
                Incomes = incomes.Select(IncomeMapper.ToResponse).ToList()
            });
    }
}
