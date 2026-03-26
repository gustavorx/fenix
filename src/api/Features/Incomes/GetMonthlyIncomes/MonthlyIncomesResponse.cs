using api.Features.Incomes.Shared;

namespace api.Features.Incomes.GetMonthlyIncomes;

public class MonthlyIncomesResponse
{
    public int Month { get; init; }
    public int Year { get; init; }
    public decimal TotalAmount { get; init; }
    public IReadOnlyCollection<IncomeResponse> Incomes { get; init; } = [];
}
