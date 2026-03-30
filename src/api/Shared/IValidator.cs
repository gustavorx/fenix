namespace api.Shared;

public interface IValidator<in T>
{
    IReadOnlyList<AppError> Validate(T input);
}
