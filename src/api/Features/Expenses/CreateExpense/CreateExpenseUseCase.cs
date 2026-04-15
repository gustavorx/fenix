using api.Auth;
using api.Data;
using api.Entities;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses.CreateExpense;

public class CreateExpenseUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<CreateExpenseRequest> validator)
{
    public async Task<Result<ExpenseResponse>> ExecuteAsync(CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<ExpenseResponse>.Failure(errors);
        }

        if (request.CardId.HasValue)
        {
            var cardExists = await context.Cards
                .AnyAsync(
                    card => card.Id == request.CardId.Value && card.UserId == currentUser.UserId,
                    cancellationToken);

            if (!cardExists)
            {
                return Result<ExpenseResponse>.Failure(
                    AppError.Validation("expense.card_id.invalid", "CardId must reference a card owned by the current user."));
            }
        }

        var expense = Expense.Create(
            request.Description!,
            Money.Create(request.TotalAmount),
            request.PurchaseDate!.Value,
            request.PaymentType!.Value,
            request.TotalInstallments,
            request.FirstDueDate,
            request.CardId,
            currentUser.UserId);

        context.Expenses.Add(expense);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponse>.Success(expense.ToResponse());
    }
}
