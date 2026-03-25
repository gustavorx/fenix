using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult ToActionResult<T>(Result<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return result.ErrorType switch
        {
            ErrorType.Validation => BadRequest(new { errors = result.Errors }),
            ErrorType.NotFound => NotFound(new { errors = result.Errors }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}
