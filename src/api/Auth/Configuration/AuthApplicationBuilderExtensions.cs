namespace api.Auth;

public static class AuthApplicationBuilderExtensions
{
    public static IApplicationBuilder UseFenixAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
