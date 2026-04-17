using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Auth;

public static class AuthTokenClaimsValidator
{
    public static Task ValidateAsync(TokenValidatedContext context)
    {
        var subject = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var email = context.Principal?.FindFirst(JwtRegisteredClaimNames.Email)?.Value;

        if (!Guid.TryParse(subject, out _) || string.IsNullOrWhiteSpace(email))
        {
            context.Fail("The authentication token is missing required claims.");
        }

        return Task.CompletedTask;
    }
}
