using api.Features.Expenses.CreateExpense;
using api.Features.Expenses.GetAllExpenses;
using api.Features.Expenses.GetExpenseById;
using api.Features.Expenses.GetMonthlyExpenses;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController(
    CreateExpenseUseCase createExpenseUseCase,
    GetMonthlyExpensesUseCase getMonthlyExpensesUseCase,
    GetAllExpensesUseCase getAllExpensesUseCase,
    GetExpenseByIdUseCase getExpenseByIdUseCase) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest(AppError.Validation("expense.request.invalid", "Invalid data."));
        }

        var result = await createExpenseUseCase.ExecuteAsync(request, cancellationToken);
        return ToActionResult(
            result,
            response => CreatedAtAction(nameof(GetExpenseById), new { id = response.Id }, response));
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken)
    {
        var expenses = await getAllExpensesUseCase.ExecuteAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyExpenses(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        var result = await getMonthlyExpensesUseCase.ExecuteAsync(month, year, cancellationToken);
        return ToActionResult(result, Ok);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        var result = await getExpenseByIdUseCase.ExecuteAsync(id, cancellationToken);
        return ToActionResult(result, Ok);
    }

    private IActionResult ToActionResult<T>(Result<T> result, Func<T, IActionResult> onSuccess)
    {
        if (result.IsSuccess)
        {
            return onSuccess(result.Value!);
        }

        return result.Error!.Type switch
        {
            ErrorType.Validation => BadRequest(result.Error),
            ErrorType.NotFound => NotFound(result.Error),
            _ => BadRequest(result.Error)
        };
    }
}
