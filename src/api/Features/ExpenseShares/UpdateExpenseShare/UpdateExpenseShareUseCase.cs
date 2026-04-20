using api.Auth;
using api.Data;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShares.UpdateExpenseShare;

public class UpdateExpenseShareUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<UpdateExpenseShareRequest> validator)
{
    public async Task<Result<ExpenseShareResponse>> ExecuteAsync(
        Guid expenseId,
        Guid shareId,
        UpdateExpenseShareRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<ExpenseShareResponse>.Failure(errors);
        }

        var personId = request.PersonId!.Value;

        var share = await context.ExpenseShares
            .Include(item => item.Expense)
            .Include(item => item.Installments)
            .FirstOrDefaultAsync(
                item =>
                    item.Id == shareId &&
                    item.ExpenseId == expenseId &&
                    item.Expense.UserId == currentUser.UserId,
                cancellationToken);

        if (share == null)
        {
            return Result<ExpenseShareResponse>.Failure(
                AppError.NotFound("expense_share.not_found", "Expense share not found."));
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

        share.AssignPerson(person);

        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseShareResponse>.Success(share.ToResponse());
    }
}
