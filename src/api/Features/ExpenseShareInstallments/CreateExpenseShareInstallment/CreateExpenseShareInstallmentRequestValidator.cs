using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.CreateExpenseShareInstallment;

public sealed class CreateExpenseShareInstallmentRequestValidator : IValidator<CreateExpenseShareInstallmentRequest>
{
    public IReadOnlyList<AppError> Validate(CreateExpenseShareInstallmentRequest request)
    {
        var errors = new List<AppError>();

        if (request.Amount is null)
        {
            errors.Add(AppError.Validation(
                "expense_share_installment.amount.required",
                "Amount is required."));
        }
        else
        {
            if (request.Amount <= 0)
            {
                errors.Add(AppError.Validation(
                    "expense_share_installment.amount.invalid",
                    "Amount must be greater than zero."));
            }

            if (!Money.HasValidScale(request.Amount.Value))
            {
                errors.Add(AppError.Validation(
                    "expense_share_installment.amount.scale",
                    "Amount must have at most 2 decimal places."));
            }
        }

        if (request.DueDate is null)
        {
            errors.Add(AppError.Validation(
                "expense_share_installment.due_date.required",
                "DueDate is required."));
        }

        return errors;
    }
}
