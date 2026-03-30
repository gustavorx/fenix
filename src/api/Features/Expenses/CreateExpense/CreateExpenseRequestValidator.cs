using api.Features.Expenses.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.CreateExpense;

public sealed class CreateExpenseRequestValidator : IValidator<CreateExpenseRequest>
{
    public IReadOnlyList<AppError> Validate(CreateExpenseRequest request)
    {
        var errors = new List<AppError>();

        ValidateDescription(request, errors);
        ValidateTotalAmount(request, errors);
        ValidatePaymentType(request, errors);
        ValidateTotalInstallments(request, errors);

        return errors;
    }

    private static void ValidateDescription(CreateExpenseRequest request, List<AppError> errors)
    {
        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("expense.description.required", "Description is required."));
        }
    }

    private static void ValidateTotalAmount(CreateExpenseRequest request, List<AppError> errors)
    {
        if (request.TotalAmount <= 0)
        {
            errors.Add(AppError.Validation("expense.total_amount.invalid", "TotalAmount must be greater than zero."));
        }

        if (!Money.HasValidScale(request.TotalAmount))
        {
            errors.Add(AppError.Validation("expense.total_amount.scale", "TotalAmount must have at most 2 decimal places."));
        }
    }

    private static void ValidatePaymentType(CreateExpenseRequest request, List<AppError> errors)
    {
        var paymentType = request.PaymentType;
        var hasValidPaymentType = paymentType != null
            && Enum.IsDefined(paymentType.Value);

        if (!hasValidPaymentType)
        {
            errors.Add(AppError.Validation("expense.payment_type.invalid", "PaymentType must be 1 (Cash) or 2 (Installment)."));
        }
    }

    private static void ValidateTotalInstallments(CreateExpenseRequest request, List<AppError> errors)
    {
        var paymentType = request.PaymentType;
        var hasValidPaymentType = paymentType != null
            && Enum.IsDefined(paymentType.Value);

        if (!hasValidPaymentType)
        {
            return;
        }

        var totalInstallments = ExpenseRules.ResolveTotalInstallments(paymentType!.Value, request.TotalInstallments);
        if (totalInstallments == null)
        {
            errors.Add(AppError.Validation(
                "expense.total_installments.invalid",
                "TotalInstallments must be 1 for cash or greater than 1 for installment."));
        }
    }
}
