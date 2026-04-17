using Microsoft.Extensions.Options;

namespace api.Auth;

public sealed class AuthOptionsValidator : IValidateOptions<AuthOptions>
{
    public ValidateOptionsResult Validate(string? name, AuthOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            failures.Add("Auth:Issuer is required.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            failures.Add("Auth:Audience is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            failures.Add("Auth:SigningKey is required.");
        }
        else if (options.SigningKey.Length < 32)
        {
            failures.Add("Auth:SigningKey must have at least 32 characters.");
        }

        if (options.TokenLifetimeMinutes <= 0)
        {
            failures.Add("Auth:TokenLifetimeMinutes must be greater than zero.");
        }

        var seenEmails = new HashSet<string>(StringComparer.Ordinal);
        for (var index = 0; index < options.SeedUsers.Count; index++)
        {
            var seedUser = options.SeedUsers[index];

            if (string.IsNullOrWhiteSpace(seedUser.Name))
            {
                failures.Add($"Auth:SeedUsers:{index}:Name is required.");
            }

            if (string.IsNullOrWhiteSpace(seedUser.Email))
            {
                failures.Add($"Auth:SeedUsers:{index}:Email is required.");
            }

            if (string.IsNullOrWhiteSpace(seedUser.Password))
            {
                failures.Add($"Auth:SeedUsers:{index}:Password is required.");
            }

            if (!string.IsNullOrWhiteSpace(seedUser.Email))
            {
                var normalizedEmail = EmailAddressNormalizer.Normalize(seedUser.Email);
                if (!seenEmails.Add(normalizedEmail))
                {
                    failures.Add($"Auth:SeedUsers contains duplicated email '{normalizedEmail}'.");
                }
            }
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
