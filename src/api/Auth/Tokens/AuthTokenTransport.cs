using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Auth;

public static class AuthTokenTransport
{
    public static Task ResolveAsync(MessageReceivedContext context)
    {
        var authorizationHeader = context.Request.Headers.Authorization.FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(authorizationHeader) &&
            authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            context.Token = authorizationHeader["Bearer ".Length..].Trim();
            return Task.CompletedTask;
        }

        if (context.Request.Cookies.TryGetValue(AuthCookieNames.AuthToken, out var token) &&
            !string.IsNullOrWhiteSpace(token))
        {
            context.Token = token;
        }

        return Task.CompletedTask;
    }
}
