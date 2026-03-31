namespace api.Auth;

public interface ICurrentUser
{
    Guid UserId { get; }
    string Email { get; }
}
