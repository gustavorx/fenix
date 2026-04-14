using api.Auth;
using api.Data;
using api.Features.Incomes.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.UpdateIncome;

public class UpdateIncomeUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<UpdateIncomeRequest> validator)
{
    public async Task<Result<IncomeResponse>> ExecuteAsync(
        Guid id,
        UpdateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<IncomeResponse>.Failure(errors);
        }

        var income = await context.Incomes
            .FirstOrDefaultAsync(item => item.Id == id && item.UserId == currentUser.UserId, cancellationToken);

        if (income == null)
        {
            return Result<IncomeResponse>.Failure(
                AppError.NotFound("income.not_found", "Income not found."));
        }

        income.Update(
            request.Description ?? income.Description,
            request.Amount.HasValue ? Money.Create(request.Amount.Value) : income.Amount,
            request.ReceivedDate ?? income.ReceivedDate);

        await context.SaveChangesAsync(cancellationToken);

        return Result<IncomeResponse>.Success(income.ToResponse());
    }
}
