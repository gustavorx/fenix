using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShares.DeleteExpenseShare;

public class DeleteExpenseShareUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(
        Guid expenseId,
        Guid shareId,
        CancellationToken cancellationToken)
    {
        var share = await context.ExpenseShares
            .Include(item => item.Expense)
            .FirstOrDefaultAsync(
                item =>
                    item.Id == shareId &&
                    item.ExpenseId == expenseId &&
                    item.Expense.UserId == currentUser.UserId,
                cancellationToken);

        if (share == null)
        {
            return Result.Failure(
                AppError.NotFound("expense_share.not_found", "Expense share not found."));
        }

        context.ExpenseShares.Remove(share);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
