using api.Features.People.Shared;
using api.Shared;

namespace api.Features.People.UpdatePerson;

public sealed class UpdatePersonRequestValidator : IValidator<UpdatePersonRequest>
{
    public IReadOnlyList<AppError> Validate(UpdatePersonRequest request)
    {
        var errors = new List<AppError>();

        if (request.Name is null)
        {
            errors.Add(AppError.Validation("person.update.empty", "At least one field with a value must be provided."));
            return errors;
        }

        PersonValidation.ValidateName(request.Name, errors);

        return errors;
    }
}
