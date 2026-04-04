using api.Auth;
using api.Data;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetMonthlyExpenses;

public class GetMonthlyExpensesUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<MonthlyExpensesResponse>> ExecuteAsync(int month, int year, CancellationToken cancellationToken)
    {
        if (month is < 1 or > 12)
        {
            return Result<MonthlyExpensesResponse>.Failure(
                AppError.Validation("expense.month.invalid", "Month must be between 1 and 12."));
        }

        if (year is < 1 or > 9999)
        {
            return Result<MonthlyExpensesResponse>.Failure(
                AppError.Validation("expense.year.invalid", "Year must be between 1 and 9999."));
        }

        var installments = await context.Installments
            .AsNoTracking()
            .Include(installment => installment.Expense)
            .Where(installment =>
                installment.Expense.UserId == currentUser.UserId &&
                installment.DueDate.Month == month &&
                installment.DueDate.Year == year)
            .OrderBy(installment => installment.DueDate)
            .ThenBy(installment => installment.Number)
            .ToListAsync(cancellationToken);

        return Result<MonthlyExpensesResponse>.Success(
            new MonthlyExpensesResponse
            {
                Month = month,
                Year = year,
                TotalAmount = installments.Aggregate(Money.Zero, (total, installment) => total + installment.Amount).Value,
                Installments = installments.Select(installment => installment.ToMonthlyInstallmentResponse()).ToList()
            });
    }
}
