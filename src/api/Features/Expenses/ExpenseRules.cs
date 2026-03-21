namespace api.Features.Expenses;

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

    public static IReadOnlyList<decimal> SplitAmount(decimal totalAmount, int totalInstallments)
    {
        var baseAmount = Math.Floor((totalAmount / totalInstallments) * 100) / 100;
        var amounts = Enumerable.Repeat(baseAmount, totalInstallments).ToArray();
        var remainder = totalAmount - (baseAmount * totalInstallments);

        for (var index = 0; remainder > 0; index = (index + 1) % totalInstallments)
        {
            amounts[index] += 0.01m;
            remainder -= 0.01m;
        }

        return amounts;
    }

    public static DateTime NormalizeDate(DateTime? date)
    {
        if (date == null)
        {
            return DateTime.UtcNow;
        }

        return date.Value.Kind switch
        {
            DateTimeKind.Utc => date.Value,
            DateTimeKind.Local => date.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(date.Value, DateTimeKind.Utc)
        };
    }
}
