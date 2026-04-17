using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Auth.Models;
using api.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Auth;

public sealed class JwtTokenService(
    IOptions<AuthOptions> authOptionsAccessor,
    TimeProvider timeProvider) : IJwtTokenService
{
    private readonly AuthOptions _authOptions = authOptionsAccessor.Value;

    public AuthToken CreateToken(User user)
    {
        var issuedAt = timeProvider.GetUtcNow();
        var expiresAt = issuedAt.AddMinutes(_authOptions.TokenLifetimeMinutes);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.SigningKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _authOptions.Issuer,
            audience: _authOptions.Audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: signingCredentials);

        var encodedToken = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthToken(encodedToken, expiresAt);
    }
}
