using api.Auth;
using api.Features.Auth.Login;
using api.Features.Auth.Me;
using api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(
    LoginUseCase loginUseCase,
    GetCurrentUserUseCase getCurrentUserUseCase,
    IHostEnvironment hostEnvironment) : ApiControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest? request, CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("auth.login.request.invalid", "Invalid data."));
        }

        var result = await loginUseCase.ExecuteAsync(request, cancellationToken);

        return ToActionResult(result, response =>
        {
            Response.Cookies.Append(
                AuthCookieNames.AuthToken,
                response.Token,
                BuildCookieOptions(response.ExpiresAt));

            return Ok(response);
        });
    }

    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(
            AuthCookieNames.AuthToken,
            new CookieOptions
            {
                Path = "/"
            });

        return NoContent();
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var result = await getCurrentUserUseCase.ExecuteAsync(cancellationToken);

        return ToActionResult(result, Ok);
    }

    private CookieOptions BuildCookieOptions(DateTimeOffset expiresAt)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            SameSite = SameSiteMode.Lax,
            Path = "/",
            Secure = !hostEnvironment.IsDevelopment(),
            Expires = expiresAt
        };
    }
}
