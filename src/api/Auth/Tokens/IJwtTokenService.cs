using api.Entities;
using api.Auth.Models;

namespace api.Auth;

public interface IJwtTokenService
{
    AuthToken CreateToken(User user);
}
