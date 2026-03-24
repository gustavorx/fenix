using api.Entities;

namespace api.Features.Expenses.Shared;

public static class ExpenseMapper
{
    public static ExpenseResponse ToResponse(this Expense expense)
    {
        var orderedInstallments = expense.Installments
            .OrderBy(installment => installment.Number)
            .ToList();

        return new ExpenseResponse
        {
            Id = expense.Id,
            Description = expense.Description,
            TotalAmount = expense.TotalAmount.Value,
            PurchaseDate = expense.Date,
            PaymentType = expense.Type,
            TotalInstallments = expense.InstallmentsQuantity ?? orderedInstallments.Count,
            Installments = orderedInstallments.Select(ToInstallmentResponse).ToList()
        };
    }

    public static InstallmentResponse ToInstallmentResponse(Installment installment)
    {
        return new InstallmentResponse
        {
            Id = installment.Id,
            Number = installment.Number,
            Amount = installment.Amount.Value,
            DueDate = installment.DueDate,
            Paid = installment.Paid
        };
    }
}
