namespace api.Shared;

public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, IReadOnlyList<AppError> errors)
    {
        IsSuccess = isSuccess;
        Value = value;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public IReadOnlyList<AppError> Errors { get; }
    public ErrorType? ErrorType => IsFailure ? Errors[0].Type : null;

    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(params AppError[] errors) => Failure((IEnumerable<AppError>)errors);

    public static Result<T> Failure(IEnumerable<AppError> errors)
    {
        var errorList = errors.ToArray();

        if (errorList.Length == 0)
        {
            throw new InvalidOperationException("Failure result must contain at least one error.");
        }

        var distinctTypes = errorList
            .Select(error => error.Type)
            .Distinct()
            .ToArray();

        if (distinctTypes.Length > 1)
        {
            throw new InvalidOperationException("Failure result cannot contain mixed error types.");
        }

        return new(false, default, errorList);
    }
}
