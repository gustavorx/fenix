using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.UpdateExpenseShareInstallment;

public sealed class UpdateExpenseShareInstallmentRequestValidator : IValidator<UpdateExpenseShareInstallmentRequest>
{
    public IReadOnlyList<AppError> Validate(UpdateExpenseShareInstallmentRequest request)
    {
        var errors = new List<AppError>();

        if (request.Amount is not null)
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

        return errors;
    }
}
