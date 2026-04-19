using api.Entities;

namespace api.Features.People.Shared;

public static class PersonMapper
{
    public static PersonResponse ToResponse(this Person person)
    {
        return new PersonResponse
        {
            Id = person.Id,
            Name = person.Name
        };
    }
}
