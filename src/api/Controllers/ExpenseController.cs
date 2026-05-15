using api.Features.Expenses.CreateExpense;
using api.Features.Expenses.DeleteExpense;
using api.Features.Expenses.GetAllExpenses;
using api.Features.Expenses.GetExpenseById;
using api.Features.Expenses.GetMonthlyExpenses;
using api.Features.Expenses.Shared;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expenses")]
[Produces("application/json")]
public class ExpenseController(
    CreateExpenseUseCase createExpenseUseCase,
    DeleteExpenseUseCase deleteExpenseUseCase,
    GetMonthlyExpensesUseCase getMonthlyExpensesUseCase,
    GetAllExpensesUseCase getAllExpensesUseCase,
    GetExpenseByIdUseCase getExpenseByIdUseCase) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("expense.request.invalid", "Invalid data.")
            );
        }

        var result = await createExpenseUseCase.ExecuteAsync(request, cancellationToken);
        
        return ToActionResult(
            result,
            response => CreatedAtAction(nameof(GetExpenseById), new { id = response.Id }, response));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ExpenseResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken)
    {
        var expenses = await getAllExpensesUseCase.ExecuteAsync(cancellationToken);
        
        return Ok(expenses);
    }

    [HttpGet("monthly")]
    [ProducesResponseType(typeof(MonthlyExpensesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetMonthlyExpenses(
        [FromQuery] GetMonthlyExpensesRequest request,
        CancellationToken cancellationToken)
    {
        var result = await getMonthlyExpensesUseCase.ExecuteAsync(request, cancellationToken);
        
        return ToActionResult(result, Ok);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ExpenseDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getExpenseByIdUseCase.ExecuteAsync(id, cancellationToken);
        
        return ToActionResult(result, Ok);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpense(Guid id, CancellationToken cancellationToken)
    {
        var result = await deleteExpenseUseCase.ExecuteAsync(id, cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
