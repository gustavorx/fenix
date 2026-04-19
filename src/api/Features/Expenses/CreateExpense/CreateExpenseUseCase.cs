using api.Auth;
using api.Data;
using api.Entities;
using api.Enums;
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

        var sharePersonIds = request.Shares?
            .Select(share => share.PersonId!.Value)
            .Distinct()
            .ToArray() ?? [];

        if (sharePersonIds.Length > 0)
        {
            var ownedPeopleCount = await context.People
                .Where(person => person.UserId == currentUser.UserId && sharePersonIds.Contains(person.Id))
                .CountAsync(cancellationToken);

            if (ownedPeopleCount != sharePersonIds.Length)
            {
                return Result<ExpenseResponse>.Failure(
                    AppError.Validation(
                        "expense.shares.person_id.invalid",
                        "Each share PersonId must reference a person owned by the current user."));
            }
        }

        var expense = request.InstallmentCreateMode switch
        {
            InstallmentCreateMode.Generated => Expense.Create(
                request.Description!,
                Money.Create(request.TotalAmount!.Value),
                request.PurchaseDate!.Value,
                request.PaymentType!.Value,
                request.TotalInstallments,
                request.FirstDueDate,
                request.CardId,
                currentUser.UserId),
            InstallmentCreateMode.Explicit => Expense.Create(
                request.Description!,
                request.PurchaseDate!.Value,
                request.PaymentType!.Value,
                request.Installments!
                    .Select(installment => new ExpenseInstallmentDraft(
                        Money.Create(installment.Amount!.Value),
                        installment.DueDate!.Value))
                    .ToList(),
                request.CardId,
                currentUser.UserId),
            _ => throw new InvalidOperationException("InstallmentCreateMode must be validated before executing the use case.")
        };

        if (request.Shares is { Count: > 0 })
        {
            foreach (var share in request.Shares)
            {
                expense.AddShare(ExpenseShare.Create(
                    expense.Id,
                    share.PersonId!.Value,
                    share.Installments!
                        .Select(installment => new ExpenseShareInstallmentDraft(
                            Money.Create(installment.Amount!.Value),
                            installment.DueDate!.Value))
                        .ToList()));
            }
        }

        context.Expenses.Add(expense);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponse>.Success(expense.ToResponse());
    }
}
