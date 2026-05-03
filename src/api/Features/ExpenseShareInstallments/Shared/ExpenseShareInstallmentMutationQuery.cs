using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShareInstallments.Shared;

public static class ExpenseShareInstallmentMutationQuery
{
    public static Task<ExpenseShare?> FindOwnedShareForInstallmentMutationAsync(
        this IQueryable<ExpenseShare> shares,
        Guid expenseId,
        Guid shareId,
        Guid userId,
        CancellationToken cancellationToken)
    {
        return shares
            .Include(share => share.Expense)
                .ThenInclude(expense => expense.Shares)
            .Include(share => share.Person)
            .Include(share => share.Installments)
            .FirstOrDefaultAsync(
                share =>
                    share.Id == shareId &&
                    share.ExpenseId == expenseId &&
                    share.Expense.UserId == userId,
                cancellationToken);
    }
}
