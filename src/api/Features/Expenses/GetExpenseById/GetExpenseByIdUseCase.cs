using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetExpenseById;

public class GetExpenseByIdUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<ExpenseDetailResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var expense = await context.Expenses
            .AsNoTracking()
            .AsSplitQuery()
            .Include(item => item.Installments)
            .Include(item => item.Shares)
                .ThenInclude(share => share.Person)
            .Include(item => item.Shares)
                .ThenInclude(share => share.Installments)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (expense == null)
        {
            return Result<ExpenseDetailResponse>.Failure(
                AppError.NotFound("expense.not_found", "Expense not found."));
        }

        return Result<ExpenseDetailResponse>.Success(expense.ToDetailResponse());
    }
}
