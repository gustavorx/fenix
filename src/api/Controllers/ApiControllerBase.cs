using api.Shared;
using api.Observability;
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

        TrackErrors(result.Errors);

        return result.ErrorType switch
        {
            ErrorType.Validation => BadRequest(new { errors = result.Errors }),
            ErrorType.NotFound => NotFound(new { errors = result.Errors }),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }

    protected IActionResult BadRequestWithErrors(params AppError[] errors)
    {
        TrackErrors(errors);

        return BadRequest(new { errors });
    }

    protected void TrackErrors(IEnumerable<AppError> errors)
    {
        var errorList = errors.ToArray();

        if (errorList.Length == 0)
        {
            return;
        }

        HttpContext.Items[ObservabilityHttpContextItemKeys.ErrorType] = errorList[0].Type.ToString();
        HttpContext.Items[ObservabilityHttpContextItemKeys.ErrorCodes] = errorList
            .Select(error => error.Code)
            .ToArray();
    }
}
