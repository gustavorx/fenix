using api.Shared;
using api.Features.ExpenseShares.Shared;

namespace api.Features.ExpenseShares.CreateExpenseShare;

public sealed class CreateExpenseShareRequestValidator : IValidator<CreateExpenseShareRequest>
{
    public IReadOnlyList<AppError> Validate(CreateExpenseShareRequest request)
    {
        var errors = new List<AppError>();

        if (request.PersonId is null)
        {
            errors.Add(AppError.Validation("expense_share.person_id.required", "PersonId is required."));
        }
        else if (request.PersonId == Guid.Empty)
        {
            errors.Add(AppError.Validation("expense_share.person_id.invalid", "PersonId must be a valid identifier."));
        }

        ExpenseShareInstallmentRequestValidation.ValidateInstallments(
            request.Installments,
            "expense_share.installments",
            "Installments",
            installment => installment.Amount,
            installment => installment.DueDate,
            errors);

        return errors;
    }
}
