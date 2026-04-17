namespace api.Features.Auth.Login;

public sealed class LoginRequest
{
    public string? Email { get; init; }
    public string? Password { get; init; }
}
