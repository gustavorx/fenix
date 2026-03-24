using api.Data;
using api.Features.Incomes.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.GetAllIncomes;

public class GetAllIncomesUseCase(FenixContext context)
{
    public async Task<IReadOnlyCollection<IncomeResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var incomes = await context.Incomes
            .AsNoTracking()
            .OrderByDescending(income => income.Date)
            .ToListAsync(cancellationToken);
        
        return incomes.Select(IncomeMapper.ToResponse).ToList();
    }
}