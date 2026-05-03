using System.Text.Json.Serialization;

namespace api.Features.ExpenseShareInstallments.UpdateExpenseShareInstallment;

public class UpdateExpenseShareInstallmentRequest
{
    private DateOnly? paidDate;

    public decimal? Amount { get; set; }
    public DateOnly? DueDate { get; set; }

    public DateOnly? PaidDate
    {
        get => paidDate;
        set
        {
            paidDate = value;
            HasPaidDateChange = true;
        }
    }

    [JsonIgnore]
    public bool HasPaidDateChange { get; private set; }
}
