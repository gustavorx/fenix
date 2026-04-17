using System.Text;
using api.Entities;
using api.Features.Auth.Login;
using api.Features.Auth.Me;
using api.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Auth;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddFenixAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authOptionsSection = configuration.GetSection(AuthOptions.SectionName);
        var authOptions = authOptionsSection.Get<AuthOptions>() ?? new AuthOptions();

        AddAuthOptions(services, authOptionsSection);
        AddAuthCoreServices(services);
        AddJwtAuthentication(services, authOptions);
        AddAuthorization(services);
        AddAuthUseCases(services);

        return services;
    }

    private static void AddAuthOptions(IServiceCollection services, IConfigurationSection authOptionsSection)
    {
        services.AddSingleton<IValidateOptions<AuthOptions>, AuthOptionsValidator>();
        services.AddOptions<AuthOptions>()
            .Bind(authOptionsSection)
            .ValidateOnStart();
    }

    private static void AddAuthCoreServices(IServiceCollection services)
    {
        services.AddSingleton<TimeProvider>(TimeProvider.System);
        services.AddHttpContextAccessor();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
    }

    private static void AddJwtAuthentication(IServiceCollection services, AuthOptions authOptions)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options => ConfigureJwtBearer(options, authOptions));
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
        });
    }

    private static void AddAuthUseCases(IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
        services.AddScoped<GetCurrentUserUseCase>();
    }

    private static void ConfigureJwtBearer(JwtBearerOptions options, AuthOptions authOptions)
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = authOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = authOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = AuthTokenTransport.ResolveAsync,
            OnTokenValidated = AuthTokenClaimsValidator.ValidateAsync,
            OnChallenge = AuthChallengeResponse.HandleAsync
        };
    }
}
