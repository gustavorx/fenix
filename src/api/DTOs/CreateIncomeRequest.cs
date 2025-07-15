namespace api.DTOs;

public class CreateIncomeRequest
{
    public string Description { get; set; }
    public decimal Amount { get; set; }
    public Guid UserId { get; set; }
}