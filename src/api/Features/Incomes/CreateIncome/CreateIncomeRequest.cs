namespace api.Features.Incomes.CreateIncome;

public class CreateIncomeRequest
{
    public string? Description { get; set; }
    public decimal Amount { get; set; }
    public DateOnly? ReceivedDate { get; set; }
}
