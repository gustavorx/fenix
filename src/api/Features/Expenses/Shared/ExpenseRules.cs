using api.Entities;
using api.ValueObjects;

namespace api.Features.Expenses.Shared;

public static class ExpenseRules
{
    public static int? ResolveTotalInstallments(ExpensePaymentType paymentType, int? totalInstallments)
    {
        if (paymentType == ExpensePaymentType.Cash)
        {
            return 1;
        }

        if (totalInstallments is > 1)
        {
            return totalInstallments.Value;
        }

        return null;
    }

    public static IReadOnlyList<Money> SplitAmount(Money totalAmount, int totalInstallments)
    {
        var totalCents = decimal.ToInt64(totalAmount.Value * 100m);
        var baseCents = totalCents / totalInstallments;
        var remainderCents = totalCents % totalInstallments;
        var amounts = new List<Money>(totalInstallments);

        for (var index = 0; index < totalInstallments; index++)
        {
            var installmentCents = baseCents + (index < remainderCents ? 1 : 0);
            amounts.Add(Money.Create(installmentCents / 100m));
        }

        return amounts;
    }
}
