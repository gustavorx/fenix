using api.Auth;
using api.Data;
using api.Entities;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShares.CreateExpenseShare;

public class CreateExpenseShareUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<CreateExpenseShareRequest> validator)
{
    public async Task<Result<ExpenseShareResponse>> ExecuteAsync(
        Guid expenseId,
        CreateExpenseShareRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<ExpenseShareResponse>.Failure(errors);
        }

        var personId = request.PersonId!.Value;

        var expense = await context.Expenses
            .Include(item => item.Shares)
            .FirstOrDefaultAsync(
                item => item.Id == expenseId && item.UserId == currentUser.UserId,
                cancellationToken);

        if (expense == null)
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.NotFound("expense.not_found", "Expense not found."));
        }

        var person = await context.People
            .FirstOrDefaultAsync(
                item => item.Id == personId && item.UserId == currentUser.UserId,
                cancellationToken);

        if (person == null)
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.Validation(
                    "expense_share.person_id.invalid",
                    "PersonId must reference a person owned by the current user."));
        }

        var shareInstallmentDrafts = request.Installments!.ToShareInstallmentDrafts(
            installment => installment.Amount,
            installment => installment.DueDate);
        var shareTotal = shareInstallmentDrafts
            .Select(installment => installment.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);
        var totalSharedAmount = expense.Shares
            .Select(share => share.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);

        if ((totalSharedAmount + shareTotal).Value > expense.TotalAmount.Value)
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.Validation(
                    "expense_share.total.exceeded",
                    "The total shared amount must be less than or equal to the expense total amount."));
        }

        var share = ExpenseShare.Create(expense.Id, person, shareInstallmentDrafts);
        expense.AddShare(share);

        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseShareResponse>.Success(share.ToResponse());
    }
}
