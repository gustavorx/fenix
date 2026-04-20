using api.Entities;

namespace api.Features.ExpenseShares.Shared;

public static class ExpenseShareMapper
{
    public static ExpenseShareResponse ToResponse(this ExpenseShare share)
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

    private static ExpenseShareInstallmentResponse ToExpenseShareInstallmentResponse(
        ExpenseShareInstallment installment)
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
