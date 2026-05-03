namespace api.ValueObjects;

public readonly record struct ExpenseShareInstallmentDraft(Money Amount, DateOnly DueDate);
