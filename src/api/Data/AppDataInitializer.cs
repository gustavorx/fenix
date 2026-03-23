using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public static class AppDataInitializer
{
    public static readonly Guid DefaultUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string DefaultUserEmail = "default@fenix.local";

    public static async Task InitializeAsync(FenixContext context)
    {
        await context.Database.MigrateAsync();

        var userExists = await context.Users.AnyAsync(user => user.Id == DefaultUserId);
        if (userExists)
        {
            return;
        }

        context.Users.Add(new User
        {
            Id = DefaultUserId,
            Name = "Default User",
            Email = DefaultUserEmail,
            PasswordHash = "bootstrap-user"
        });

        await context.SaveChangesAsync();
    }
}
