using api.Auth;
using api.Data;
using api.Entities;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;

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

        var expense = Expense.Create(
            request.Description!,
            Money.Create(request.TotalAmount),
            request.PurchaseDate!.Value,
            request.PaymentType!.Value,
            request.TotalInstallments,
            request.FirstDueDate,
            currentUser.UserId);

        context.Expenses.Add(expense);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponse>.Success(expense.ToResponse());
    }
}
