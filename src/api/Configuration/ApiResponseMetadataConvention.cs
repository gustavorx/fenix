using api.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace api.Configuration;

public sealed class ApiResponseMetadataConvention : IActionModelConvention
{
    public void Apply(ActionModel action)
    {
        if (AllowsAnonymous(action))
        {
            return;
        }

        action.Filters.Add(new ProducesResponseTypeAttribute(
            typeof(ApiErrorResponse),
            StatusCodes.Status401Unauthorized));
    }

    private static bool AllowsAnonymous(ActionModel action) =>
        action.Attributes.OfType<AllowAnonymousAttribute>().Any()
        || action.Controller.Attributes.OfType<AllowAnonymousAttribute>().Any();
}
