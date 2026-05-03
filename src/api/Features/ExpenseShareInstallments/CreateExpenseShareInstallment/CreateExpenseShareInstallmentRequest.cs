namespace api.Features.ExpenseShareInstallments.CreateExpenseShareInstallment;

public class CreateExpenseShareInstallmentRequest
{
    public decimal? Amount { get; set; }
    public DateOnly? DueDate { get; set; }
}
