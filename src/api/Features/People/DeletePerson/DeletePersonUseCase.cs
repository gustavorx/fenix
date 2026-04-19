using api.Auth;
using api.Data;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.People.DeletePerson;

public class DeletePersonUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var person = await context.People
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (person == null)
        {
            return Result.Failure(
                AppError.NotFound("person.not_found", "Person not found."));
        }

        context.People.Remove(person);
        await context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
