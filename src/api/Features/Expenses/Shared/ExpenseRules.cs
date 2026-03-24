using api.ValueObjects;

namespace api.Features.Expenses.Shared;

public static class ExpenseRules
{
    public static string? NormalizePaymentType(string paymentType)
    {
        if (string.IsNullOrWhiteSpace(paymentType))
        {
            return null;
        }

        return paymentType.Trim().ToLowerInvariant() switch
        {
            "cash" => "Cash",
            "avista" => "Cash",
            "a vista" => "Cash",
            "installment" => "Installment",
            "parcelado" => "Installment",
            _ => null
        };
    }

    public static int? ResolveTotalInstallments(string paymentType, int? totalInstallments)
    {
        if (paymentType == "Cash")
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
