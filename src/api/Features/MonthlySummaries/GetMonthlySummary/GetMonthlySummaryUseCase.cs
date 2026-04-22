using api.Auth;
using api.Data;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.MonthlySummaries.GetMonthlySummary;

public class GetMonthlySummaryUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<GetMonthlySummaryRequest> validator)
{
    public async Task<Result<MonthlySummaryResponse>> ExecuteAsync(
        GetMonthlySummaryRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<MonthlySummaryResponse>.Failure(errors);
        }

        var month = request.Month!.Value;
        var year = request.Year!.Value;
        var period = MonthPeriod.Create(month, year);

        var incomeAmounts = await context.Incomes
            .AsNoTracking()
            .Where(income =>
                income.UserId == currentUser.UserId &&
                income.ReceivedDate >= period.StartDate &&
                income.ReceivedDate <= period.EndDate)
            .Select(income => income.Amount)
            .ToListAsync(cancellationToken);

        var grossExpenseAmounts = await context.Installments
            .AsNoTracking()
            .Where(installment =>
                installment.Expense.UserId == currentUser.UserId &&
                installment.DueDate >= period.StartDate &&
                installment.DueDate <= period.EndDate)
            .Select(installment => installment.Amount)
            .ToListAsync(cancellationToken);

        var sharedReceivableAmounts = await context.ExpenseShareInstallments
            .AsNoTracking()
            .Where(installment =>
                installment.ExpenseShare.Expense.UserId == currentUser.UserId &&
                installment.DueDate >= period.StartDate &&
                installment.DueDate <= period.EndDate)
            .Select(installment => installment.Amount)
            .ToListAsync(cancellationToken);

        var totalIncomes = Sum(incomeAmounts);
        var totalGrossExpenses = Sum(grossExpenseAmounts);
        var totalSharedReceivables = Sum(sharedReceivableAmounts);
        var totalNetExpenses = totalGrossExpenses - totalSharedReceivables;
        var myFinalBalance = totalIncomes - totalNetExpenses;

        return Result<MonthlySummaryResponse>.Success(
            new MonthlySummaryResponse
            {
                Month = month,
                Year = year,
                Totals = new MonthlySummaryTotalsResponse
                {
                    TotalIncomes = totalIncomes.Value,
                    TotalSharedReceivables = totalSharedReceivables.Value,
                    TotalGrossExpenses = totalGrossExpenses.Value,
                    TotalNetExpenses = totalNetExpenses.Value,
                    MyFinalBalance = myFinalBalance.Value
                }
            });
    }

    private static Money Sum(IEnumerable<Money> amounts)
    {
        return amounts.Aggregate(Money.Zero, (total, amount) => total + amount);
    }
}
