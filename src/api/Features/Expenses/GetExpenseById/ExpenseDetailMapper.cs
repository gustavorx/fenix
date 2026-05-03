using api.Entities;
using api.Features.Expenses.Shared;

namespace api.Features.Expenses.GetExpenseById;

public static class ExpenseDetailMapper
{
    public static ExpenseDetailResponse ToDetailResponse(this Expense expense)
    {
        return new ExpenseDetailResponse
        {
            Id = expense.Id,
            Description = expense.Description,
            TotalAmount = expense.TotalAmount.Value,
            PurchaseDate = expense.PurchaseDate,
            PaymentType = expense.PaymentType,
            TotalInstallments = expense.InstallmentsQuantity,
            CardId = expense.CardId,
            Installments = expense.Installments
                .OrderBy(installment => installment.Number)
                .Select(ExpenseMapper.ToInstallmentResponse)
                .ToList(),
            Shares = expense.Shares
                .OrderBy(share => share.PersonId == null ? 1 : 0)
                .ThenBy(share => share.Person == null ? null : share.Person.Name)
                .ThenBy(share => share.Id)
                .Select(ToExpenseShareResponse)
                .ToList()
        };
    }

    private static ExpenseShareResponse ToExpenseShareResponse(ExpenseShare share)
    {
        return new ExpenseShareResponse
        {
            Id = share.Id,
            PersonId = share.PersonId,
            PersonName = share.Person?.Name,
            Amount = share.Amount.Value,
            PaidAmount = share.PaidAmount.Value,
            OutstandingAmount = share.OutstandingAmount.Value,
            IsFullyPaid = share.IsFullyPaid,
            Installments = share.Installments
                .OrderBy(installment => installment.DueDate)
                .ThenBy(installment => installment.Id)
                .Select(ToExpenseShareInstallmentResponse)
                .ToList()
        };
    }

    private static ExpenseShareInstallmentResponse ToExpenseShareInstallmentResponse(ExpenseShareInstallment installment)
    {
        return new ExpenseShareInstallmentResponse
        {
            Id = installment.Id,
            Amount = installment.Amount.Value,
            DueDate = installment.DueDate,
            PaidDate = installment.PaidDate,
            IsPaid = installment.IsPaid
        };
    }
}
