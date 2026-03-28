using api.Entities;
using api.Features.Expenses.GetMonthlyExpenses;

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
            PurchaseDate = expense.PurchaseDate,
            PaymentType = expense.PaymentType,
            TotalInstallments = expense.InstallmentsQuantity,
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

    public static MonthlyExpenseInstallmentResponse ToMonthlyInstallmentResponse(this Installment installment)
    {
        return new MonthlyExpenseInstallmentResponse
        {
            InstallmentId = installment.Id,
            ExpenseId = installment.ExpenseId,
            Description = installment.Expense.Description,
            PaymentType = installment.Expense.PaymentType,
            TotalAmount = installment.Expense.TotalAmount.Value,
            TotalInstallments = installment.Expense.InstallmentsQuantity,
            InstallmentNumber = installment.Number,
            InstallmentAmount = installment.Amount.Value,
            PurchaseDate = installment.Expense.PurchaseDate,
            DueDate = installment.DueDate,
            Paid = installment.Paid
        };
    }
}
