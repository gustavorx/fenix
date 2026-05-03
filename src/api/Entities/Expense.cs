using api.ValueObjects;

namespace api.Entities;

public class Expense
{
    private Expense()
    {
        Description = null!;
        User = null!;
    }

    private Expense(
        string description,
        Money totalAmount,
        DateOnly purchaseDate,
        ExpensePaymentType paymentType,
        int installmentsQuantity,
        Guid? cardId,
        Guid userId)
    {
        Id = Guid.NewGuid();
        Description = description;
        TotalAmount = totalAmount;
        PurchaseDate = purchaseDate;
        PaymentType = paymentType;
        InstallmentsQuantity = installmentsQuantity;
        CardId = cardId;
        UserId = userId;
        User = null!;
    }

    public Guid Id { get; private set; }
    public string Description { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateOnly PurchaseDate { get; private set; }
    public ExpensePaymentType PaymentType { get; private set; }
    public int InstallmentsQuantity { get; private set; }

    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public Guid? CardId { get; private set; }
    public Card? Card { get; private set; }

    public ICollection<Installment> Installments { get; private set; } = [];
    public ICollection<ExpenseShare> Shares { get; private set; } = [];

    public static Expense Create(
        string description,
        Money totalAmount,
        DateOnly purchaseDate,
        ExpensePaymentType paymentType,
        int? requestedInstallments,
        DateOnly? firstDueDate,
        Guid? cardId,
        Guid userId)
    {
        var normalizedDescription = description.Trim();
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (totalAmount.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(totalAmount), "TotalAmount must be greater than zero.");
        }

        if (!TryResolveInstallmentsQuantity(paymentType, requestedInstallments, out var installmentsQuantity))
        {
            throw new ArgumentException(
                "TotalInstallments must be 1 for cash or greater than 1 for installment.",
                nameof(requestedInstallments));
        }

        var expense = new Expense(
            normalizedDescription!,
            totalAmount,
            purchaseDate,
            paymentType,
            installmentsQuantity,
            cardId,
            userId);

        expense.Installments = expense.CreateInstallments(totalAmount, firstDueDate ?? purchaseDate, installmentsQuantity);
        return expense;
    }

    public static Expense Create(
        string description,
        DateOnly purchaseDate,
        ExpensePaymentType paymentType,
        IReadOnlyCollection<ExpenseInstallmentDraft> installments,
        Guid? cardId,
        Guid userId)
    {
        var normalizedDescription = description.Trim();
        if (string.IsNullOrWhiteSpace(normalizedDescription))
        {
            throw new ArgumentException("Description is required.", nameof(description));
        }

        if (installments.Count == 0)
        {
            throw new ArgumentException("Installments are required.", nameof(installments));
        }

        var totalAmount = installments
            .Select(installment => installment.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);

        if (totalAmount.Value <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(installments), "TotalAmount must be greater than zero.");
        }

        var expense = new Expense(
            normalizedDescription!,
            totalAmount,
            purchaseDate,
            paymentType,
            installments.Count,
            cardId,
            userId);

        expense.Installments = installments
            .Select((installment, index) => Installment.Create(expense.Id, index + 1, installment.Amount, installment.DueDate))
            .ToList();

        return expense;
    }

    public static bool TryResolveInstallmentsQuantity(
        ExpensePaymentType paymentType,
        int? requestedInstallments,
        out int installmentsQuantity)
    {
        if (paymentType == ExpensePaymentType.Cash)
        {
            installmentsQuantity = 1;
            return true;
        }

        if (paymentType == ExpensePaymentType.Installment && requestedInstallments is > 1)
        {
            installmentsQuantity = requestedInstallments.Value;
            return true;
        }

        installmentsQuantity = 0;
        return false;
    }

    public void AddShare(ExpenseShare share)
    {
        ArgumentNullException.ThrowIfNull(share);

        if (share.ExpenseId != Id)
        {
            throw new ArgumentException("Share must belong to the current expense.", nameof(share));
        }

        Shares.Add(share);
    }

    private ICollection<Installment> CreateInstallments(Money totalAmount, DateOnly firstDueDate, int installmentsQuantity)
    {
        var installmentAmounts = SplitAmount(totalAmount, installmentsQuantity);

        return installmentAmounts
            .Select((amount, index) => Installment.Create(Id, index + 1, amount, firstDueDate.AddMonths(index)))
            .ToList();
    }

    private static IReadOnlyList<Money> SplitAmount(Money totalAmount, int installmentsQuantity)
    {
        var totalCents = decimal.ToInt64(totalAmount.Value * 100m);
        var baseCents = totalCents / installmentsQuantity;
        var remainderCents = totalCents % installmentsQuantity;
        var amounts = new List<Money>(installmentsQuantity);

        for (var index = 0; index < installmentsQuantity; index++)
        {
            var installmentCents = baseCents + (index < remainderCents ? 1 : 0);
            amounts.Add(Money.Create(installmentCents / 100m));
        }

        return amounts;
    }
}
