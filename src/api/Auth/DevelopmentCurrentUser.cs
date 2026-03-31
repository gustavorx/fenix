namespace api.Auth;

public class DevelopmentCurrentUser : ICurrentUser
{
    public Guid UserId => DevelopmentUser.Id;
    public string Email => DevelopmentUser.Email;
}

public static class DevelopmentUser
{
    public static readonly Guid Id = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string Email = "default@fenix.local";
    public const string Name = "Default User";
}