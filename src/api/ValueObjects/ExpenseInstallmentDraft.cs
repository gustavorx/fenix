namespace api.ValueObjects;

public readonly record struct ExpenseInstallmentDraft(Money Amount, DateOnly DueDate);
