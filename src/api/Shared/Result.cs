namespace api.Shared;

public sealed class Result<T>
{
    private Result(bool isSuccess, T? value, AppError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public bool IsSuccess { get; }
    public T? Value { get; }
    public AppError? Error { get; }

    public static Result<T> Success(T value) => new(true, value, null);

    public static Result<T> Failure(AppError error) => new(false, default, error);
}
