using api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expense-share-installments")]
public class ExpenseShareInstallmentMonthlyController(
    GetMonthlyExpenseShareInstallmentsUseCase getMonthlyExpenseShareInstallmentsUseCase) : ApiControllerBase
{
    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyExpenseShareInstallments(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result = await getMonthlyExpenseShareInstallmentsUseCase.ExecuteAsync(
            month,
            year,
            cancellationToken);

        return ToActionResult(result, Ok);
    }
}
