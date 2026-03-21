using api.Data;
using api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses;

public class ExpenseQueries(FenixContext context)
{
    public async Task<IReadOnlyCollection<ExpenseResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var expenses = await context.Expenses
            .AsNoTracking()
            .Include(expense => expense.Installments)
            .OrderByDescending(expense => expense.Date)
            .ToListAsync(cancellationToken);

        return expenses.Select(ExpenseMapper.ToResponse).ToList();
    }

    public async Task<ExpenseResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var expense = await context.Expenses
            .AsNoTracking()
            .Include(item => item.Installments)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return expense == null ? null : ExpenseMapper.ToResponse(expense);
    }
}
