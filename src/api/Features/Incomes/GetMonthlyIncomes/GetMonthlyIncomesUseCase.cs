using api.Auth;
using api.Data;
using api.Features.Incomes.Shared;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Incomes.GetMonthlyIncomes;

public class GetMonthlyIncomesUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<GetMonthlyIncomesRequest> validator)
{
    public async Task<Result<MonthlyIncomesResponse>> ExecuteAsync(
        GetMonthlyIncomesRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<MonthlyIncomesResponse>.Failure(errors);
        }

        var period = MonthPeriod.Create(request.Month, request.Year);

        var incomes = await context.Incomes
            .AsNoTracking()
            .Where(income =>
                income.UserId == currentUser.UserId &&
                income.ReceivedDate >= period.StartDate &&
                income.ReceivedDate <= period.EndDate)
            .OrderByDescending(income => income.ReceivedDate)
            .ToListAsync(cancellationToken);

        return Result<MonthlyIncomesResponse>.Success(
            new MonthlyIncomesResponse
            {
                Month = request.Month,
                Year = request.Year,
                TotalAmount = incomes.Aggregate(Money.Zero, (total, income) => total + income.Amount).Value,
                Incomes = incomes.Select(IncomeMapper.ToResponse).ToList()
            });
    }
}
