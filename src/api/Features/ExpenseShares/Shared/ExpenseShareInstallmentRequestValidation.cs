using api.Shared;
using api.ValueObjects;

namespace api.Features.ExpenseShares.Shared;

public static class ExpenseShareInstallmentRequestValidation
{
    public static decimal? ValidateInstallments<TInstallment>(
        IReadOnlyCollection<TInstallment>? installments,
        string collectionPath,
        string collectionDisplayName,
        Func<TInstallment, decimal?> getAmount,
        Func<TInstallment, DateOnly?> getDueDate,
        List<AppError> errors)
    {
        if (installments is not { Count: > 0 })
        {
            errors.Add(AppError.Validation(
                $"{collectionPath}.required",
                $"{collectionDisplayName} must contain at least one item."));
            return null;
        }

        var canComputeTotal = true;
        var totalAmount = 0m;

        for (var index = 0; index < installments.Count; index++)
        {
            var installment = installments.ElementAt(index);
            var pathIndex = index + 1;
            var amount = getAmount(installment);
            var dueDate = getDueDate(installment);

            if (amount is null)
            {
                errors.Add(AppError.Validation(
                    $"{collectionPath}[{pathIndex}].amount.required",
                    $"{collectionDisplayName}[{pathIndex}].Amount is required."));
                canComputeTotal = false;
            }
            else
            {
                if (amount <= 0)
                {
                    errors.Add(AppError.Validation(
                        $"{collectionPath}[{pathIndex}].amount.invalid",
                        $"{collectionDisplayName}[{pathIndex}].Amount must be greater than zero."));
                    canComputeTotal = false;
                }

                if (!Money.HasValidScale(amount.Value))
                {
                    errors.Add(AppError.Validation(
                        $"{collectionPath}[{pathIndex}].amount.scale",
                        $"{collectionDisplayName}[{pathIndex}].Amount must have at most 2 decimal places."));
                    canComputeTotal = false;
                }

                if (amount > 0 && Money.HasValidScale(amount.Value))
                {
                    totalAmount += amount.Value;
                }
            }

            if (dueDate is null)
            {
                errors.Add(AppError.Validation(
                    $"{collectionPath}[{pathIndex}].due_date.required",
                    $"{collectionDisplayName}[{pathIndex}].DueDate is required."));
            }
        }

        return canComputeTotal ? totalAmount : null;
    }
}
