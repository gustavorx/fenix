using api.ValueObjects;

namespace api.Entities;

public class ExpenseShare
{
    private ExpenseShare()
    {
        Expense = null!;
    }

    private ExpenseShare(Guid expenseId, Guid personId, IReadOnlyCollection<ExpenseShareInstallmentDraft> installments)
    {
        if (installments.Count == 0)
        {
            throw new ArgumentException("Installments are required.", nameof(installments));
        }

        Id = Guid.NewGuid();
        ExpenseId = expenseId;
        PersonId = personId;
        Amount = installments
            .Select(installment => installment.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);
        Installments = installments
            .Select(installment => ExpenseShareInstallment.Create(Id, installment.Amount, installment.DueDate))
            .ToList();
        Expense = null!;
    }

    private ExpenseShare(Guid expenseId, Person person, IReadOnlyCollection<ExpenseShareInstallmentDraft> installments)
        : this(expenseId, person.Id, installments)
    {
        Person = person;
    }

    public Guid Id { get; private set; }
    public Money Amount { get; private set; }

    public Guid ExpenseId { get; private set; }
    public Expense Expense { get; private set; }

    public Guid? PersonId { get; private set; }
    public Person? Person { get; private set; }

    public ICollection<ExpenseShareInstallment> Installments { get; private set; } = [];

    public Money PaidAmount => Installments
        .Where(installment => installment.IsPaid)
        .Aggregate(Money.Zero, (current, installment) => current + installment.Amount);

    public Money OutstandingAmount => Amount - PaidAmount;

    public bool IsFullyPaid => OutstandingAmount.Value == 0m;

    public static ExpenseShare Create(
        Guid expenseId,
        Guid personId,
        IReadOnlyCollection<ExpenseShareInstallmentDraft> installments)
    {
        return new ExpenseShare(expenseId, personId, installments);
    }

    public static ExpenseShare Create(
        Guid expenseId,
        Person person,
        IReadOnlyCollection<ExpenseShareInstallmentDraft> installments)
    {
        ArgumentNullException.ThrowIfNull(person);

        return new ExpenseShare(expenseId, person, installments);
    }

    public void AssignPerson(Guid personId)
    {
        if (personId == Guid.Empty)
        {
            throw new ArgumentException("PersonId must be a valid identifier.", nameof(personId));
        }

        PersonId = personId;
    }

    public void AssignPerson(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        AssignPerson(person.Id);
        Person = person;
    }

    public ExpenseShareInstallment AddInstallment(Money amount, DateOnly dueDate)
    {
        var installment = ExpenseShareInstallment.Create(Id, amount, dueDate);

        Installments.Add(installment);
        RecalculateAmount();

        return installment;
    }

    public void UpdateInstallment(
        Guid installmentId,
        Money? amount,
        DateOnly? dueDate,
        DateOnly? paidDate,
        bool updatePaidDate)
    {
        var installment = GetInstallment(installmentId);

        installment.Update(amount, dueDate, paidDate, updatePaidDate);
        RecalculateAmount();
    }

    public ExpenseShareInstallment RemoveInstallment(Guid installmentId)
    {
        if (Installments.Count <= 1)
        {
            throw new InvalidOperationException("Share must contain at least one installment.");
        }

        var installment = GetInstallment(installmentId);

        Installments.Remove(installment);
        RecalculateAmount();

        return installment;
    }

    private ExpenseShareInstallment GetInstallment(Guid installmentId)
    {
        return Installments.FirstOrDefault(installment => installment.Id == installmentId)
            ?? throw new InvalidOperationException("Share installment not found.");
    }

    private void RecalculateAmount()
    {
        Amount = Installments
            .Select(installment => installment.Amount)
            .Aggregate(Money.Zero, (current, amount) => current + amount);
    }
}
