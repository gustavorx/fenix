namespace api.DTOs;

public class CreateIncomeRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? Date { get; set; }
}
