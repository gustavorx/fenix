using api.DTOs;
using api.Features.Expenses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController(
    CreateExpenseUseCase createExpenseUseCase,
    MonthlyExpensesQuery monthlyExpensesQuery,
    ExpenseQueries expenseQueries) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateExpense(
        [FromBody] CreateExpenseRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequest("Invalid data.");
        }

        var result = await createExpenseUseCase.ExecuteAsync(request, cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(result.ErrorMessage);
        }

        return CreatedAtAction(nameof(GetExpenseById), new { id = result.Response!.Id }, result.Response);
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses(CancellationToken cancellationToken)
    {
        var expenses = await expenseQueries.ListAsync(cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyExpenses(
        [FromQuery] int month,
        [FromQuery] int year,
        CancellationToken cancellationToken)
    {
        if (month is < 1 or > 12)
        {
            return BadRequest("Month must be between 1 and 12.");
        }

        if (year is < 1 or > 9999)
        {
            return BadRequest("Year must be between 1 and 9999.");
        }

        var response = await monthlyExpensesQuery.ExecuteAsync(month, year, cancellationToken);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpenseById(Guid id, CancellationToken cancellationToken)
    {
        var expense = await expenseQueries.GetByIdAsync(id, cancellationToken);
        if (expense == null)
        {
            return NotFound();
        }

        return Ok(expense);
    }
}
