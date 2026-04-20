using api.Auth;
using api.Data;
using api.Features.ExpenseShareInstallments.Shared;
using api.Shared;

namespace api.Features.ExpenseShareInstallments.DeleteExpenseShareInstallment;

public class DeleteExpenseShareInstallmentUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(
        Guid expenseId,
        Guid shareId,
        Guid installmentId,
        CancellationToken cancellationToken)
    {
        var share = await context.ExpenseShares.FindOwnedShareForInstallmentMutationAsync(
            expenseId,
            shareId,
            currentUser.UserId,
            cancellationToken);

        if (share == null)
        {
            return Result.Failure(
                AppError.NotFound("expense_share.not_found", "Expense share not found."));
        }

        if (share.Installments.All(installment => installment.Id != installmentId))
        {
            return Result.Failure(
                AppError.NotFound("expense_share_installment.not_found", "Expense share installment not found."));
        }

        if (share.Installments.Count <= 1)
        {
            return Result.Failure(
                AppError.Validation(
                    "expense_share_installment.last_installment",
                    "The last share installment cannot be deleted. Delete the share instead."));
        }

        var removedInstallment = share.RemoveInstallment(installmentId);
        context.ExpenseShareInstallments.Remove(removedInstallment);

        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
