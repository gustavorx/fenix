using api.Shared;

namespace api.Features.Auth.Login;

public sealed class LoginRequestValidator : IValidator<LoginRequest>
{
    public IReadOnlyList<AppError> Validate(LoginRequest request)
    {
        var errors = new List<AppError>();

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            errors.Add(AppError.Validation("auth.login.email.required", "Email is required."));
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            errors.Add(AppError.Validation("auth.login.password.required", "Password is required."));
        }

        return errors;
    }
}
