using api.Data;
using api.Features.Expenses.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetExpenseById;

public class GetExpenseByIdUseCase(FenixContext context)
{
    public async Task<ExpenseResponse?> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var expense = await context.Expenses
            .AsNoTracking()
            .Include(item => item.Installments)
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        return expense == null ? null : ExpenseMapper.ToResponse(expense);
    }
}
