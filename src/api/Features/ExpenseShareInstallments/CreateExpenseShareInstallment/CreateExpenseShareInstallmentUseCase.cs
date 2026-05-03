using api.Auth;
using api.Data;
using api.Features.ExpenseShareInstallments.Shared;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.CreateExpenseShareInstallment;

public class CreateExpenseShareInstallmentUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<CreateExpenseShareInstallmentRequest> validator)
{
    public async Task<Result<ExpenseShareResponse>> ExecuteAsync(
        Guid expenseId,
        Guid shareId,
        CreateExpenseShareInstallmentRequest request,
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

        var installmentAmount = Money.Create(request.Amount!.Value);
        var updatedShareAmount = share.Amount + installmentAmount;

        if (!ExpenseShareInstallmentTotalValidation.KeepsExpenseSharedTotalWithinLimit(share, updatedShareAmount))
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.Validation(
                    "expense_share.total.exceeded",
                    "The total shared amount must be less than or equal to the expense total amount."));
        }

        share.AddInstallment(installmentAmount, request.DueDate!.Value);

        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseShareResponse>.Success(share.ToResponse());
    }
}
