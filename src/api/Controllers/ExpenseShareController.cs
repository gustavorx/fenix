using api.Features.ExpenseShares.CreateExpenseShare;
using api.Features.ExpenseShares.DeleteExpenseShare;
using api.Features.ExpenseShares.UpdateExpenseShare;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expenses/{expenseId:guid}/shares")]
public class ExpenseShareController(
    CreateExpenseShareUseCase createExpenseShareUseCase,
    UpdateExpenseShareUseCase updateExpenseShareUseCase,
    DeleteExpenseShareUseCase deleteExpenseShareUseCase) : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateExpenseShare(
        Guid expenseId,
        [FromBody] CreateExpenseShareRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("expense_share.request.invalid", "Invalid data.")
            );
        }

        var result = await createExpenseShareUseCase.ExecuteAsync(expenseId, request, cancellationToken);

        return ToActionResult(
            result,
            response => CreatedAtAction(
                nameof(ExpenseController.GetExpenseById),
                "Expense",
                new { id = expenseId },
                response));
    }

    [HttpPatch("{shareId:guid}")]
    public async Task<IActionResult> UpdateExpenseShare(
        Guid expenseId,
        Guid shareId,
        [FromBody] UpdateExpenseShareRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("expense_share.request.invalid", "Invalid data.")
            );
        }

        var result = await updateExpenseShareUseCase.ExecuteAsync(expenseId, shareId, request, cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpDelete("{shareId:guid}")]
    public async Task<IActionResult> DeleteExpenseShare(
        Guid expenseId,
        Guid shareId,
        CancellationToken cancellationToken)
    {
        var result = await deleteExpenseShareUseCase.ExecuteAsync(expenseId, shareId, cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
