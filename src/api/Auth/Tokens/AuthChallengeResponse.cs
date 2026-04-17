using System.Text.Json;
using api.Observability;
using api.Shared;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace api.Auth;

public static class AuthChallengeResponse
{
    public static Task HandleAsync(JwtBearerChallengeContext context)
    {
        if (context.Response.HasStarted)
        {
            return Task.CompletedTask;
        }

        context.HandleResponse();

        var error = AppError.Unauthorized("auth.unauthorized", "Authentication is required.");
        Track(context.HttpContext, error);

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        context.Response.ContentType = "application/json";

        return JsonSerializer.SerializeAsync(context.Response.Body, new { errors = new[] { error } });
    }

    private static void Track(HttpContext context, params AppError[] errors)
    {
        context.Items[ObservabilityHttpContextItemKeys.ErrorType] = errors[0].Type.ToString();
        context.Items[ObservabilityHttpContextItemKeys.ErrorCodes] = errors
            .Select(error => error.Code)
            .ToArray();
    }
}
