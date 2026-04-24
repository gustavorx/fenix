using api.Auth;
using api.Data;
using api.Entities;
using api.Features.People.Shared;
using api.Shared;

namespace api.Features.People.CreatePerson;

public class CreatePersonUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<CreatePersonRequest> validator)
{
    public async Task<Result<PersonResponse>> ExecuteAsync(
        CreatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<PersonResponse>.Failure(errors);
        }

        var person = Person.Create(request.Name!, currentUser.UserId);

        context.People.Add(person);
        await context.SaveChangesAsync(cancellationToken);

        return Result<PersonResponse>.Success(person.ToResponse());
    }
}
