using api.Entities;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.CreateExpense;

public sealed class CreateExpenseRequestValidator : IValidator<CreateExpenseRequest>
{
    public IReadOnlyList<AppError> Validate(CreateExpenseRequest request)
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

        if (request.PurchaseDate is null)
        {
            errors.Add(AppError.Validation("expense.purchase_date.required", "PurchaseDate is required."));
        }

        if (request.CardId == Guid.Empty)
        {
            errors.Add(AppError.Validation("expense.card_id.invalid", "CardId must be a valid identifier."));
        }

        var paymentType = request.PaymentType;
        var hasValidPaymentType = paymentType != null && Enum.IsDefined(paymentType.Value);
        if (!hasValidPaymentType)
        {
            errors.Add(AppError.Validation("expense.payment_type.invalid", "PaymentType must be 1 (Cash) or 2 (Installment)."));
        }

        if (hasValidPaymentType
            && !Expense.TryResolveInstallmentsQuantity(paymentType!.Value, request.TotalInstallments, out _))
        {
            errors.Add(AppError.Validation(
                "expense.total_installments.invalid",
                "TotalInstallments must be 1 for cash or greater than 1 for installment."));
        }

        return errors;
    }
}
