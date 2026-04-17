using api.Auth;
using api.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace api.Data;

public static class AppDataInitializer
{
    public static async Task InitializeAsync(
        FenixContext context,
        IOptions<AuthOptions> authOptionsAccessor,
        IPasswordHasher<User> passwordHasher)
    {
        await context.Database.MigrateAsync();

        var authOptions = authOptionsAccessor.Value;
        if (authOptions.SeedUsers.Count == 0)
        {
            return;
        }

        var seedEmails = authOptions.SeedUsers
            .Select(seedUser => EmailAddressNormalizer.Normalize(seedUser.Email))
            .Distinct()
            .ToArray();

        var existingEmails = await context.Users
            .Where(user => seedEmails.Contains(user.Email))
            .Select(user => user.Email)
            .ToListAsync();

        var existingEmailSet = existingEmails.ToHashSet(StringComparer.Ordinal);
        var usersToAdd = new List<User>();

        foreach (var seedUser in authOptions.SeedUsers)
        {
            var normalizedEmail = EmailAddressNormalizer.Normalize(seedUser.Email);
            if (existingEmailSet.Contains(normalizedEmail))
            {
                continue;
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = seedUser.Name.Trim(),
                Email = normalizedEmail
            };

            user.PasswordHash = passwordHasher.HashPassword(user, seedUser.Password);
            usersToAdd.Add(user);
            existingEmailSet.Add(normalizedEmail);
        }

        if (usersToAdd.Count == 0)
        {
            return;
        }

        context.Users.AddRange(usersToAdd);
        await context.SaveChangesAsync();
    }
}
