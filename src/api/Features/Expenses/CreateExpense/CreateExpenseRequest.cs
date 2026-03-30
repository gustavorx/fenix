using api.Entities;

namespace api.Features.Expenses.CreateExpense;

public class CreateExpenseRequest
{
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public ExpensePaymentType? PaymentType { get; set; }
    public int? TotalInstallments { get; set; }
    public DateOnly? FirstDueDate { get; set; }
}
