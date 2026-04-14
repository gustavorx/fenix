using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.DeleteExpense;

public class DeleteExpenseUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var expense = await context.Expenses
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (expense == null)
        {
            return Result.Failure(
                AppError.NotFound("expense.not_found", "Expense not found."));
        }

        context.Expenses.Remove(expense);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
