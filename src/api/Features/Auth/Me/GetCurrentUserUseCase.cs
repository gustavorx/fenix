using api.Auth;
using api.Data;
using api.Features.Auth.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Auth.Me;

public sealed class GetCurrentUserUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<AuthUserResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == currentUser.UserId, cancellationToken);

        if (user is null)
        {
            return Result<AuthUserResponse>.Failure(
                AppError.Unauthorized("auth.user.invalid", "Authentication is invalid."));
        }

        return Result<AuthUserResponse>.Success(user.ToAuthUserResponse());
    }
}
