using api.Features.MonthlySummaries.GetMonthlySummary;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/monthly-summary")]
[Produces("application/json")]
public class MonthlySummaryController(GetMonthlySummaryUseCase getMonthlySummaryUseCase) : ApiControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(MonthlySummaryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlySummary(
        [FromQuery] GetMonthlySummaryRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("monthly_summary.request.invalid", "Invalid data.")
            );
        }

        var result = await getMonthlySummaryUseCase.ExecuteAsync(request, cancellationToken);

        return ToActionResult(result, Ok);
    }
}
