using api.Shared;

namespace api.Features.ExpenseShares.UpdateExpenseShare;

public sealed class UpdateExpenseShareRequestValidator : IValidator<UpdateExpenseShareRequest>
{
    public IReadOnlyList<AppError> Validate(UpdateExpenseShareRequest request)
    {
        var errors = new List<AppError>();

        if (request.PersonId is null)
        {
            errors.Add(AppError.Validation(
                "expense_share.person_id.required",
                "PersonId is required."));
        }
        else if (request.PersonId == Guid.Empty)
        {
            errors.Add(AppError.Validation(
                "expense_share.person_id.invalid",
                "PersonId must be a valid identifier."));
        }

        return errors;
    }
}
