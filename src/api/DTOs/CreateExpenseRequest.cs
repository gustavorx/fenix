namespace api.DTOs;

public class CreateExpenseRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateTime? Date { get; set; }
    public string PaymentType { get; set; } = string.Empty;
    public int? InstallmentsQuantity { get; set; }
    public DateTime? FirstDueDate { get; set; }
}
