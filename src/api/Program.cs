using System.Diagnostics;
using api.Auth;
using api.Configuration;
using api.Data;
using api.Entities;
using api.Observability;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.Configure(options =>
{
    options.ActivityTrackingOptions =
        ActivityTrackingOptions.TraceId |
        ActivityTrackingOptions.SpanId |
        ActivityTrackingOptions.ParentId;
});

builder.Services.AddDbContext<FenixContext>((serviceProvider, options) =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString(("DefaultConnection")))
        .AddInterceptors(serviceProvider.GetRequiredService<DatabaseCommandMetricsInterceptor>()));

builder.Services.AddFenixAuth(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
builder.Services.AddFenixObservability(builder.Configuration);

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FenixContext>();
    var configuredAuthOptions = scope.ServiceProvider.GetRequiredService<IOptions<AuthOptions>>();
    var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

    await AppDataInitializer.InitializeAsync(context, configuredAuthOptions, passwordHasher);
}

app.UseFenixMetricsEndpoint();
app.UseMiddleware<RequestObservabilityMiddleware>();
app.UseFenixAuth();

app.MapControllers();

app.Run();
