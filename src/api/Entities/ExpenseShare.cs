namespace api.Entities;

public class ExpenseShare
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public bool Paid { get; set; }
    public DateTime? PaymentDate { get; set; }

    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; }

    public Guid? PersonId { get; set; } // null = the user themself
    public Person Person { get; set; }
}
