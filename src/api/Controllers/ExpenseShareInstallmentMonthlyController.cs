using api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expense-share-installments")]
[Produces("application/json")]
public class ExpenseShareInstallmentMonthlyController(
    GetMonthlyExpenseShareInstallmentsUseCase getMonthlyExpenseShareInstallmentsUseCase) : ApiControllerBase
{
    [HttpGet("monthly")]
    [ProducesResponseType(typeof(MonthlyExpenseShareInstallmentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlyExpenseShareInstallments(
        [FromQuery] GetMonthlyExpenseShareInstallmentsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getMonthlyExpenseShareInstallmentsUseCase.ExecuteAsync(
            request,
            cancellationToken);

        return ToActionResult(result, Ok);
    }
}
