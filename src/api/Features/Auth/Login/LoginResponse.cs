using api.Features.Auth.Shared;

namespace api.Features.Auth.Login;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; init; }
    public AuthUserResponse User { get; init; } = new();
}
