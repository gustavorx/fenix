using api.Entities;
using api.Features.ExpenseShares.Shared;
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
                .Select(ExpenseShareMapper.ToResponse)
                .ToList()
        };
    }
}
