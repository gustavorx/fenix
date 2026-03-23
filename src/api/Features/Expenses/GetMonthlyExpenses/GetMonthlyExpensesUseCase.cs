using api.Data;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetMonthlyExpenses;

public class GetMonthlyExpensesUseCase(FenixContext context)
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
            .Where(installment => installment.DueDate.Month == month && installment.DueDate.Year == year)
            .OrderBy(installment => installment.DueDate)
            .ThenBy(installment => installment.Number)
            .ToListAsync(cancellationToken);

        return Result<MonthlyExpensesResponse>.Success(
            new MonthlyExpensesResponse
            {
                Month = month,
                Year = year,
                TotalAmount = installments.Aggregate(Money.Zero, (total, installment) => total + installment.Amount).Value,
                Installments = installments.Select(installment => new MonthlyExpenseInstallmentResponse
                {
                    InstallmentId = installment.Id,
                    ExpenseId = installment.ExpenseId,
                    Description = installment.Expense.Description,
                    PaymentType = installment.Expense.Type,
                    TotalAmount = installment.Expense.TotalAmount.Value,
                    TotalInstallments = installment.Expense.InstallmentsQuantity ?? 1,
                    InstallmentNumber = installment.Number,
                    InstallmentAmount = installment.Amount.Value,
                    PurchaseDate = installment.Expense.Date,
                    DueDate = installment.DueDate,
                    Paid = installment.Paid
                }).ToList()
            });
    }
}
