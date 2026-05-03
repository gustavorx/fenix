using api.Entities;
using api.ValueObjects;

namespace api.Features.ExpenseShareInstallments.Shared;

public static class ExpenseShareInstallmentTotalValidation
{
    public static bool KeepsExpenseSharedTotalWithinLimit(ExpenseShare share, Money updatedShareAmount)
    {
        var currentExpenseSharedAmount = share.Expense.Shares
            .Select(expenseShare => expenseShare.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);
        var updatedExpenseSharedAmount = currentExpenseSharedAmount - share.Amount + updatedShareAmount;

        return updatedExpenseSharedAmount.Value <= share.Expense.TotalAmount.Value;
    }

    public static Money CalculateUpdatedShareAmount(
        ExpenseShare share,
        Guid installmentId,
        Money? updatedInstallmentAmount)
    {
        return share.Installments
            .Select(installment =>
                installment.Id == installmentId && updatedInstallmentAmount.HasValue
                    ? updatedInstallmentAmount.Value
                    : installment.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);
    }
}
