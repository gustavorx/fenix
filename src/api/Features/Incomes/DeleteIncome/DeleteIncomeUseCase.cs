using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.DeleteIncome;

public class DeleteIncomeUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var income = await context.Incomes
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (income == null)
        {
            return Result.Failure(
                AppError.NotFound("income.not_found", "Income not found."));
        }

        context.Incomes.Remove(income);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
