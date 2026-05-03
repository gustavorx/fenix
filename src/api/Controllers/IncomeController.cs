using api.Features.Incomes.CreateIncome;
using api.Features.Incomes.DeleteIncome;
using api.Features.Incomes.GetAllIncomes;
using api.Features.Incomes.GetIncomeById;
using api.Features.Incomes.GetMonthlyIncomes;
using api.Features.Incomes.UpdateIncome;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/incomes")]
public class IncomeController(
    CreateIncomeUseCase createIncomeUseCase,
    DeleteIncomeUseCase deleteIncomeUseCase,
    GetIncomeByIdUseCase getIncomeByIdUseCase,
    GetAllIncomesUseCase getAllIncomesUseCase,
    GetMonthlyIncomesUseCase getMonthlyIncomesUseCase,
    UpdateIncomeUseCase updateIncomeUseCase) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateIncome(
        [FromBody] CreateIncomeRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("income.request.invalid", "Invalid data.")
            );
        }

        var result = await createIncomeUseCase.ExecuteAsync(request, cancellationToken);
        
        return ToActionResult(
            result,
            response => CreatedAtAction(nameof(GetIncomeById), new { id = response.Id }, response));
    }

    [HttpGet]
    public async Task<IActionResult> GetIncomes(CancellationToken cancellationToken)
    {
        var incomes = await getAllIncomesUseCase.ExecuteAsync(cancellationToken);
        
        return Ok(incomes);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyIncomes(
        [FromQuery] GetMonthlyIncomesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getMonthlyIncomesUseCase.ExecuteAsync(request, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetIncomeById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getIncomeByIdUseCase.ExecuteAsync(id, cancellationToken);
        
        return ToActionResult(result, Ok);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateIncome(
        Guid id,
        [FromBody] UpdateIncomeRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("income.request.invalid", "Invalid data.")
            );
        }

        var result = await updateIncomeUseCase.ExecuteAsync(id, request, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteIncome(Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteIncomeUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
