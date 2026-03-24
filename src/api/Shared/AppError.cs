namespace api.Shared;

public sealed class AppError
{
    public AppError(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }
    public string Message { get; }
    public ErrorType Type { get; }

    public static AppError Validation(string code, string message) =>
        new(code, message, ErrorType.Validation);

    public static AppError NotFound(string code, string message) =>
        new(code, message, ErrorType.NotFound);
}
