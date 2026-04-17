namespace api.Auth.Models;

public sealed record AuthToken(string Value, DateTimeOffset ExpiresAt);
