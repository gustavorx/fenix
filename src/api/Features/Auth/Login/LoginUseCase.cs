using api.Auth;
using api.Data;
using api.Entities;
using api.Features.Auth.Shared;
using api.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Auth.Login;

public sealed class LoginUseCase(
    FenixContext context,
    IValidator<LoginRequest> validator,
    IPasswordHasher<User> passwordHasher,
    IJwtTokenService jwtTokenService)
{
    public async Task<Result<LoginResponse>> ExecuteAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<LoginResponse>.Failure(errors);
        }

        var normalizedEmail = EmailAddressNormalizer.Normalize(request.Email!);
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Email == normalizedEmail, cancellationToken);

        if (user is null)
        {
            return InvalidCredentials();
        }

        var passwordVerificationResult =
            passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password!);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return InvalidCredentials();
        }

        var token = jwtTokenService.CreateToken(user);

        return Result<LoginResponse>.Success(new LoginResponse
        {
            Token = token.Value,
            ExpiresAt = token.ExpiresAt,
            User = user.ToAuthUserResponse()
        });
    }

    private static Result<LoginResponse> InvalidCredentials()
    {
        return Result<LoginResponse>.Failure(
            AppError.Unauthorized("auth.credentials.invalid", "Invalid email or password."));
    }
}
