using api.Data;
using api.Features.Incomes.Shared;
using api.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.GetIncomeById;

public class GetIncomeByIdUseCase(FenixContext context)
{
    public async Task<Result<IncomeResponse>> ExecuteAsync(Guid id, CancellationToken cancellationToken)
    {
        var income = await context.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);

        if (income == null)
        {
            return Result<IncomeResponse>.Failure(
                AppError.NotFound("income.not_found", "Income not found."));
        }
        
        return Result<IncomeResponse>.Success(income.ToResponse());
    }
}