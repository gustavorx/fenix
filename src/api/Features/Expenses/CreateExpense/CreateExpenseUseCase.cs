using api.Data;
using api.Entities;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.CreateExpense;

public class CreateExpenseUseCase(
    FenixContext context,
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

        var totalAmount = Money.Create(request.TotalAmount);
        var purchaseDate = request.PurchaseDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var paymentType = request.PaymentType!.Value;
        var totalInstallments = ExpenseRules.ResolveTotalInstallments(paymentType, request.TotalInstallments)!.Value;

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            TotalAmount = totalAmount,
            PurchaseDate = purchaseDate,
            PaymentType = paymentType,
            InstallmentsQuantity = totalInstallments,
            UserId = AppDataInitializer.DefaultUserId,
            Installments = []
        };

        var firstDueDate = request.FirstDueDate ?? purchaseDate;
        var installmentAmounts = ExpenseRules.SplitAmount(totalAmount, totalInstallments);

        expense.Installments = installmentAmounts
            .Select((amount, index) => new Installment
            {
                Id = Guid.NewGuid(),
                Number = index + 1,
                Amount = amount,
                DueDate = firstDueDate.AddMonths(index),
                Paid = false,
                ExpenseId = expense.Id
            })
            .ToList();

        context.Expenses.Add(expense);
        await context.SaveChangesAsync(cancellationToken);

        return Result<ExpenseResponse>.Success(expense.ToResponse());
    }
}