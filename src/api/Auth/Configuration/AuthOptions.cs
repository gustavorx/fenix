namespace api.Auth;

public sealed class AuthOptions
{
    public const string SectionName = "Auth";

    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public string SigningKey { get; init; } = string.Empty;
    public int TokenLifetimeMinutes { get; init; } = 60;
    public List<AuthSeedUserOptions> SeedUsers { get; init; } = [];
}

public sealed class AuthSeedUserOptions
{
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
