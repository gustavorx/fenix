using api.Auth;
using api.Data;
using api.Features.People.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.People.GetPersonById;

public class GetPersonByIdUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<PersonResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var person = await context.People
            .AsNoTracking()
            .Where(item => item.Id == id && item.UserId == currentUser.UserId)
            .Select(item => new PersonResponse
            {
                Id = item.Id,
                Name = item.Name
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (person == null)
        {
            return Result<PersonResponse>.Failure(
                AppError.NotFound("person.not_found", "Person not found."));
        }

        return Result<PersonResponse>.Success(person);
    }
}
