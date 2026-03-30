using api.Data;
using api.Entities;
using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.CreateExpense;

public class CreateExpenseUseCase(FenixContext context)
{
    public async Task<Result<ExpenseResponse>> ExecuteAsync(CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<AppError>();

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("expense.description.required", "Description is required."));
        }

        if (request.TotalAmount <= 0)
        {
            errors.Add(AppError.Validation("expense.total_amount.invalid", "TotalAmount must be greater than zero."));
        }

        if (!Money.HasValidScale(request.TotalAmount))
        {
            errors.Add(AppError.Validation("expense.total_amount.scale", "TotalAmount must have at most 2 decimal places."));
        }

        var normalizedPaymentType = ExpenseRules.NormalizePaymentType(request.PaymentType);
        if (normalizedPaymentType == null)
        {
            errors.Add(AppError.Validation("expense.payment_type.invalid", "PaymentType must be 'cash' or 'installment'."));
        }

        int? totalInstallments = null;
        if (normalizedPaymentType != null)
        {
            totalInstallments = ExpenseRules.ResolveTotalInstallments(normalizedPaymentType, request.TotalInstallments);
            if (totalInstallments == null)
            {
                errors.Add(AppError.Validation(
                    "expense.total_installments.invalid",
                    "TotalInstallments must be 1 for cash or greater than 1 for installment."));
            }
        }

        if (errors.Count > 0)
        {
            return Result<ExpenseResponse>.Failure(errors);
        }

        var totalAmount = Money.Create(request.TotalAmount);
        var purchaseDate = request.PurchaseDate ?? DateOnly.FromDateTime(DateTime.UtcNow);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            TotalAmount = totalAmount,
            PurchaseDate = purchaseDate,
            Type = normalizedPaymentType!,
            InstallmentsQuantity = totalInstallments!.Value,
            UserId = AppDataInitializer.DefaultUserId,
            Installments = []
        };

        var firstDueDate = request.FirstDueDate ?? purchaseDate;
        var installmentAmounts = ExpenseRules.SplitAmount(totalAmount, totalInstallments!.Value);

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
