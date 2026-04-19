using api.Features.People.Shared;
using api.Shared;

namespace api.Features.People.CreatePerson;

public sealed class CreatePersonRequestValidator : IValidator<CreatePersonRequest>
{
    public IReadOnlyList<AppError> Validate(CreatePersonRequest request)
    {
        var errors = new List<AppError>();
        
        PersonValidation.ValidateName(request.Name, errors);

        return errors;
    }
}
