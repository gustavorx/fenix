using api.Auth;
using api.Data;
using api.Features.People.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.People.UpdatePerson;

public class UpdatePersonUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<UpdatePersonRequest> validator)
{
    public async Task<Result<PersonResponse>> ExecuteAsync(
        Guid id,
        UpdatePersonRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<PersonResponse>.Failure(errors);
        }

        var person = await context.People
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (person == null)
        {
            return Result<PersonResponse>.Failure(
                AppError.NotFound("person.not_found", "Person not found."));
        }

        person.Update(request.Name!);

        await context.SaveChangesAsync(cancellationToken);

        return Result<PersonResponse>.Success(person.ToResponse());
    }
}
