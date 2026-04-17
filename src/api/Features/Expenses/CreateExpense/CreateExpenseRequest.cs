using api.Entities;
using api.Enums;

namespace api.Features.Expenses.CreateExpense;

public class CreateExpenseRequest
{
    public string? Description { get; set; }
    public decimal? TotalAmount { get; set; }
    public DateOnly? PurchaseDate { get; set; }
    public ExpensePaymentType? PaymentType { get; set; }
    public int? TotalInstallments { get; set; }
    public DateOnly? FirstDueDate { get; set; }
    public Guid? CardId { get; set; }
    public InstallmentCreateMode? InstallmentCreateMode { get; set; }
    public IReadOnlyCollection<CreateExpenseInstallmentRequest>? Installments { get; set; }
}

public class CreateExpenseInstallmentRequest
{
    public decimal? Amount { get; set; }
    public DateOnly? DueDate { get; set; }
}
