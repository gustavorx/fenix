using api.ValueObjects;

namespace api.Features.ExpenseShares.Shared;

public static class ExpenseShareRequestMapper
{
    public static IReadOnlyCollection<ExpenseShareInstallmentDraft> ToShareInstallmentDrafts<TInstallment>(
        this IReadOnlyCollection<TInstallment> installments,
        Func<TInstallment, decimal?> getAmount,
        Func<TInstallment, DateOnly?> getDueDate)
    {
        return installments
            .Select(installment => new ExpenseShareInstallmentDraft(
                Money.Create(getAmount(installment)!.Value),
                getDueDate(installment)!.Value))
            .ToList();
    }
}
