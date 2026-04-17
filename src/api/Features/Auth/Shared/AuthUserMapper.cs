using api.Entities;

namespace api.Features.Auth.Shared;

public static class AuthUserMapper
{
    public static AuthUserResponse ToAuthUserResponse(this User user)
    {
        return new AuthUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
