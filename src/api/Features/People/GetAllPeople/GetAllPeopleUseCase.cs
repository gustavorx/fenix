using api.Auth;
using api.Data;
using api.Features.People.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.People.GetAllPeople;

public class GetAllPeopleUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<IReadOnlyList<PersonResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        return await context.People
            .AsNoTracking()
            .Where(person => person.UserId == currentUser.UserId)
            .OrderBy(person => person.Name)
            .ThenBy(person => person.Id)
            .Select(person => new PersonResponse
            {
                Id = person.Id,
                Name = person.Name
            })
            .ToListAsync(cancellationToken);
    }
}
