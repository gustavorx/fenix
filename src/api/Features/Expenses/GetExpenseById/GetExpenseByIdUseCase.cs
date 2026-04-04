using api.Auth;
using api.Data;
using api.Features.Expenses.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.GetExpenseById;

public class GetExpenseByIdUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<ExpenseResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var expense = await context.Expenses
            .AsNoTracking()
            .Include(item => item.Installments)
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (expense == null)
        {
            return Result<ExpenseResponse>.Failure(
                AppError.NotFound("expense.not_found", "Expense not found."));
        }

        return Result<ExpenseResponse>.Success(expense.ToResponse());
    }
}
