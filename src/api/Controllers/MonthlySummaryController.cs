using api.Features.MonthlySummaries.GetMonthlySummary;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/monthly-summary")]
public class MonthlySummaryController(GetMonthlySummaryUseCase getMonthlySummaryUseCase) : ApiControllerBase
{
    [HttpGet]
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
