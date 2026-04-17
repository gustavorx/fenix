using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace api.Auth;

public sealed class HttpContextCurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public Guid UserId => ResolveUserId();
    public string Email => GetRequiredClaim(JwtRegisteredClaimNames.Email);

    private Guid ResolveUserId()
    {
        var userIdClaim = GetRequiredClaim(JwtRegisteredClaimNames.Sub);
        if (Guid.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }

        throw new InvalidOperationException("Authenticated user id claim is invalid.");
    }

    private string GetRequiredClaim(string claimType)
    {
        var value = CurrentPrincipal.FindFirst(claimType)?.Value;
        if (!string.IsNullOrWhiteSpace(value))
        {
            return value;
        }

        throw new InvalidOperationException($"Authenticated user is missing required claim '{claimType}'.");
    }

    private ClaimsPrincipal CurrentPrincipal =>
        httpContextAccessor.HttpContext?.User
        ?? throw new InvalidOperationException("No active HTTP context is available for the current user.");
}
