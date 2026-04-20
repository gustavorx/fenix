using api.Auth;
using api.Data;
using api.Features.ExpenseShareInstallments.Shared;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.UpdateExpenseShareInstallment;

public class UpdateExpenseShareInstallmentUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<UpdateExpenseShareInstallmentRequest> validator)
{
    public async Task<Result<ExpenseShareResponse>> ExecuteAsync(
        Guid expenseId,
        Guid shareId,
        Guid installmentId,
        UpdateExpenseShareInstallmentRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<ExpenseShareResponse>.Failure(errors);
        }

        var share = await context.ExpenseShares.FindOwnedShareForInstallmentMutationAsync(
            expenseId,
            shareId,
            currentUser.UserId,
            cancellationToken);

        if (share == null)
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.NotFound("expense_share.not_found", "Expense share not found."));
        }

        if (share.Installments.All(installment => installment.Id != installmentId))
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.NotFound("expense_share_installment.not_found", "Expense share installment not found."));
        }

        var updatedAmount = request.Amount.HasValue ? Money.Create(request.Amount.Value) : (Money?)null;
        var updatedShareAmount = ExpenseShareInstallmentTotalValidation.CalculateUpdatedShareAmount(
            share,
            installmentId,
            updatedAmount);

        if (!ExpenseShareInstallmentTotalValidation.KeepsExpenseSharedTotalWithinLimit(share, updatedShareAmount))
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.Validation(
                    "expense_share.total.exceeded",
                    "The total shared amount must be less than or equal to the expense total amount."));
        }

        share.UpdateInstallment(
            installmentId,
            updatedAmount,
            request.DueDate,
            request.PaidDate,
            request.HasPaidDateChange);

        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseShareResponse>.Success(share.ToResponse());
    }
}
