namespace api.Entities;

public class Installment
{
    public Guid Id { get; set; }
    public int Number { get; set; } // e.g. 1,2,3
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public bool Paid { get; set; }

    public Guid ExpenseId { get; set; }
    public Expense Expense { get; set; }
}
