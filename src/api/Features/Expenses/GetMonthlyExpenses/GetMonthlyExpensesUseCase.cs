using api.Auth;
using api.Data;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetMonthlyExpenses;

public class GetMonthlyExpensesUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<GetMonthlyExpensesRequest> validator)
{
    public async Task<Result<MonthlyExpensesResponse>> ExecuteAsync(
        GetMonthlyExpensesRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<MonthlyExpensesResponse>.Failure(errors);
        }

        var period = MonthPeriod.Create(request.Month, request.Year);

        var installments = await context.Installments
            .AsNoTracking()
            .Include(installment => installment.Expense)
            .Where(installment =>
                installment.Expense.UserId == currentUser.UserId &&
                installment.DueDate >= period.StartDate &&
                installment.DueDate <= period.EndDate)
            .OrderBy(installment => installment.DueDate)
            .ThenBy(installment => installment.Number)
            .ToListAsync(cancellationToken);

        var expenseIds = installments
            .Select(installment => installment.ExpenseId)
            .Distinct()
            .ToList();

        var expenseIdsWithShares = await context.ExpenseShares
            .AsNoTracking()
            .Where(share => expenseIds.Contains(share.ExpenseId))
            .Select(share => share.ExpenseId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var expenseIdsWithSharesLookup = expenseIdsWithShares.ToHashSet();

        return Result<MonthlyExpensesResponse>.Success(
            new MonthlyExpensesResponse
            {
                Month = request.Month,
                Year = request.Year,
                TotalAmount = installments.Aggregate(Money.Zero, (total, installment) => total + installment.Amount).Value,
                Installments = installments
                    .Select(installment => installment.ToMonthlyInstallmentResponse(
                        expenseIdsWithSharesLookup.Contains(installment.ExpenseId)))
                    .ToList()
            });
    }
}
