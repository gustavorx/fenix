using api.Features.ExpenseShareInstallments.CreateExpenseShareInstallment;
using api.Features.ExpenseShareInstallments.DeleteExpenseShareInstallment;
using api.Features.ExpenseShareInstallments.UpdateExpenseShareInstallment;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers;

[ApiController]
[Route("api/expenses/{expenseId:guid}/shares/{shareId:guid}/installments")]
[Produces("application/json")]
public class ExpenseShareInstallmentController(
    CreateExpenseShareInstallmentUseCase createExpenseShareInstallmentUseCase,
    UpdateExpenseShareInstallmentUseCase updateExpenseShareInstallmentUseCase,
    DeleteExpenseShareInstallmentUseCase deleteExpenseShareInstallmentUseCase) : ApiControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ExpenseShareResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateExpenseShareInstallment(
        Guid expenseId,
        Guid shareId,
        [FromBody] CreateExpenseShareInstallmentRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("expense_share_installment.request.invalid", "Invalid data.")
            );
        }

        var result = await createExpenseShareInstallmentUseCase.ExecuteAsync(
            expenseId,
            shareId,
            request,
            cancellationToken);

        return ToActionResult(
            result,
            response => CreatedAtAction(
                nameof(ExpenseController.GetExpenseById),
                "Expense",
                new { id = expenseId },
                response));
    }

    [HttpPatch("{installmentId:guid}")]
    [ProducesResponseType(typeof(ExpenseShareResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateExpenseShareInstallment(
        Guid expenseId,
        Guid shareId,
        Guid installmentId,
        [FromBody] UpdateExpenseShareInstallmentRequest? request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            return BadRequestWithErrors(
                AppError.Validation("expense_share_installment.request.invalid", "Invalid data.")
            );
        }

        var result = await updateExpenseShareInstallmentUseCase.ExecuteAsync(
            expenseId,
            shareId,
            installmentId,
            request,
            cancellationToken);

        return ToActionResult(result, Ok);
    }

    [HttpDelete("{installmentId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteExpenseShareInstallment(
        Guid expenseId,
        Guid shareId,
        Guid installmentId,
        CancellationToken cancellationToken)
    {
        var result = await deleteExpenseShareInstallmentUseCase.ExecuteAsync(
            expenseId,
            shareId,
            installmentId,
            cancellationToken);

        return ToActionResult(result, NoContent);
    }
}
