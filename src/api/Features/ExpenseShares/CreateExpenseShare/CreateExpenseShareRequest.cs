namespace api.Features.ExpenseShares.CreateExpenseShare;

public class CreateExpenseShareRequest
{
    public Guid? PersonId { get; set; }
    public IReadOnlyCollection<CreateExpenseShareInstallmentRequest>? Installments { get; set; }
}

public class CreateExpenseShareInstallmentRequest
{
    public decimal? Amount { get; set; }
    public DateOnly? DueDate { get; set; }
}
