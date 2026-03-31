using api.Auth;
using api.Data;
using api.Features.Incomes.Shared;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.GetAllIncomes;

public class GetAllIncomesUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<IReadOnlyCollection<IncomeResponse>> ExecuteAsync(CancellationToken cancellationToken)
    {
        var incomes = await context.Incomes
            .AsNoTracking()
            .Where(income => income.UserId == currentUser.UserId)
            .OrderByDescending(income => income.ReceivedDate)
            .ToListAsync(cancellationToken);

        return incomes.Select(IncomeMapper.ToResponse).ToList();
    }
}
