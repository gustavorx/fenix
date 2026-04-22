namespace api.Features.MonthlySummaries.GetMonthlySummary;

public class MonthlySummaryResponse
{
    public int Month { get; init; }
    public int Year { get; init; }
    public MonthlySummaryTotalsResponse Totals { get; init; } = new();
}

public class MonthlySummaryTotalsResponse
{
    public decimal TotalIncomes { get; init; }
    public decimal TotalSharedReceivables { get; init; }
    public decimal TotalGrossExpenses { get; init; }
    public decimal TotalNetExpenses { get; init; }
    public decimal MyFinalBalance { get; init; }
}
