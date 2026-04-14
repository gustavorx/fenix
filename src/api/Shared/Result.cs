namespace api.Shared;

public abstract class ResultBase
{
    protected ResultBase(bool isSuccess, IReadOnlyList<AppError> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyList<AppError> Errors { get; }
    public ErrorType? ErrorType => IsFailure ? Errors[0].Type : null;

    protected static IReadOnlyList<AppError> NormalizeFailureErrors(IEnumerable<AppError> errors)
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

        return errorList;
    }
}

public sealed class Result : ResultBase
{
    private Result(bool isSuccess, IReadOnlyList<AppError> errors)
        : base(isSuccess, errors)
    {
    }

    public static Result Success() => new(true, []);

    public static Result Failure(params AppError[] errors) => Failure((IEnumerable<AppError>)errors);

    public static Result Failure(IEnumerable<AppError> errors) =>
        new(false, NormalizeFailureErrors(errors));
}

public sealed class Result<T> : ResultBase
{
    private Result(bool isSuccess, T? value, IReadOnlyList<AppError> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public T? Value { get; }

    public static Result<T> Success(T value) => new(true, value, []);

    public static Result<T> Failure(params AppError[] errors) => Failure((IEnumerable<AppError>)errors);

    public static Result<T> Failure(IEnumerable<AppError> errors) =>
        new(false, default, NormalizeFailureErrors(errors));
}
