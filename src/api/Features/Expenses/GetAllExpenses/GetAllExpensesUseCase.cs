using api.Auth;
using api.Data;
using api.Features.Expenses.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetAllExpenses;

public class GetAllExpensesUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<IReadOnlyCollection<ExpenseResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var expenses = await context.Expenses
            .AsNoTracking()
            .Where(expense => expense.UserId == currentUser.UserId)
            .Include(expense => expense.Installments)
            .OrderByDescending(expense => expense.PurchaseDate)
            .ToListAsync(cancellationToken);

        return expenses.Select(ExpenseMapper.ToResponse).ToList();
    }
}
