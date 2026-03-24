using api.Data;
using api.Features.Expenses.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetAllExpenses;

public class GetAllExpensesUseCase(FenixContext context)
{
    public async Task<IReadOnlyCollection<ExpenseResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var expenses = await context.Expenses
            .AsNoTracking()
            .Include(expense => expense.Installments)
            .OrderByDescending(expense => expense.Date)
            .ToListAsync(cancellationToken);

        return expenses.Select(ExpenseMapper.ToResponse).ToList();
    }
}
