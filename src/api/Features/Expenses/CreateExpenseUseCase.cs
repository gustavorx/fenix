using api.Data;
using api.DTOs;
using api.Entities;
using api.ValueObjects;

namespace api.Features.Expenses;

public class CreateExpenseUseCase(FenixContext context)
{
    public async Task<CreateExpenseResult> ExecuteAsync(CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return CreateExpenseResult.Failure("Description is required.");
        }

        if (request.TotalAmount <= 0)
        {
            return CreateExpenseResult.Failure("TotalAmount must be greater than zero.");
        }

        if (!Money.HasValidScale(request.TotalAmount))
        {
            return CreateExpenseResult.Failure("TotalAmount must have at most 2 decimal places.");
        }

        var normalizedPaymentType = ExpenseRules.NormalizePaymentType(request.PaymentType);
        if (normalizedPaymentType == null)
        {
            return CreateExpenseResult.Failure("PaymentType must be 'cash' or 'installment'.");
        }

        var totalInstallments = ExpenseRules.ResolveTotalInstallments(normalizedPaymentType, request.TotalInstallments);
        if (totalInstallments == null)
        {
            return CreateExpenseResult.Failure("TotalInstallments must be 1 for cash or greater than 1 for installment.");
        }

        var totalAmount = Money.Create(request.TotalAmount);

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            TotalAmount = totalAmount,
            Date = ExpenseRules.NormalizeDate(request.PurchaseDate),
            Type = normalizedPaymentType,
            InstallmentsQuantity = totalInstallments.Value,
            UserId = AppDataInitializer.DefaultUserId,
            Installments = []
        };

        var firstDueDate = ExpenseRules.NormalizeDate(request.FirstDueDate ?? request.PurchaseDate);
        var installmentAmounts = ExpenseRules.SplitAmount(totalAmount, totalInstallments.Value);

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

        return CreateExpenseResult.Success(ExpenseMapper.ToResponse(expense));
    }
}

public sealed class CreateExpenseResult
{
    private CreateExpenseResult(bool isSuccess, ExpenseResponse? response, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Response = response;
        ErrorMessage = errorMessage;
    }

    public bool IsSuccess { get; }
    public ExpenseResponse? Response { get; }
    public string? ErrorMessage { get; }

    public static CreateExpenseResult Success(ExpenseResponse response) => new(true, response, null);

    public static CreateExpenseResult Failure(string errorMessage) => new(false, null, errorMessage);
}
