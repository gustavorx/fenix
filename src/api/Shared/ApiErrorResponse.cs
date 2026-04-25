namespace api.Shared;

public sealed class ApiErrorResponse
{
    public IReadOnlyCollection<AppError> Errors { get; init; } = [];
}
