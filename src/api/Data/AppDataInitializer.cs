using api.Auth;
using api.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public static class AppDataInitializer
{
    public static async Task InitializeAsync(FenixContext context)
    {
        await context.Database.MigrateAsync();

        var userExists = await context.Users.AnyAsync(user => user.Id == DevelopmentUser.Id);
        if (userExists)
        {
            return;
        }

        context.Users.Add(new User
        {
            Id = DevelopmentUser.Id,
            Name = DevelopmentUser.Name,
            Email = DevelopmentUser.Email,
            PasswordHash = "bootstrap-user"
        });

        await context.SaveChangesAsync();
    }
}
