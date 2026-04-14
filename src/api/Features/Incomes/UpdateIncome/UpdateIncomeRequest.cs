namespace api.Features.Incomes.UpdateIncome;

public class UpdateIncomeRequest
{
    public string? Description { get; set; }
    public decimal? Amount { get; set; }
    public DateOnly? ReceivedDate { get; set; }
}
