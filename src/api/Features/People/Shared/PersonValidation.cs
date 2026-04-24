using api.Shared;

namespace api.Features.People.Shared;

public static class PersonValidation
{
    public static void ValidateName(string? name, ICollection<AppError> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            errors.Add(AppError.Validation("person.name.required", "Name is required."));
            return;
        }

        if (name.Trim().Length > 100)
        {
            errors.Add(AppError.Validation("person.name.length", "Name must have at most 100 characters."));
        }
    }
}
