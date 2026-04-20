using api.Entities;
using api.Enums;
using api.Features.ExpenseShares.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Expenses.CreateExpense;

public sealed class CreateExpenseRequestValidator : IValidator<CreateExpenseRequest>
{
    public IReadOnlyList<AppError> Validate(CreateExpenseRequest request)
    {
        var errors = new List<AppError>();

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("expense.description.required", "Description is required."));
        }

        if (request.PurchaseDate is null)
        {
            errors.Add(AppError.Validation("expense.purchase_date.required", "PurchaseDate is required."));
        }

        if (request.CardId == Guid.Empty)
        {
            errors.Add(AppError.Validation("expense.card_id.invalid", "CardId must be a valid identifier."));
        }

        var paymentType = request.PaymentType;
        var hasValidPaymentType = paymentType != null && Enum.IsDefined(paymentType.Value);
        if (!hasValidPaymentType)
        {
            errors.Add(AppError.Validation("expense.payment_type.invalid", "PaymentType must be 1 (Cash) or 2 (Installment)."));
        }

        var createMode = request.InstallmentCreateMode;
        var hasValidCreateMode = createMode != null && Enum.IsDefined(createMode.Value);
        if (!hasValidCreateMode)
        {
            errors.Add(AppError.Validation(
                "expense.installment_create_mode.invalid",
                "InstallmentCreateMode must be 1 (Generated) or 2 (Explicit)."));
            
            return errors;
        }

        decimal? expenseTotal = null;

        if (createMode == InstallmentCreateMode.Generated)
        {
            expenseTotal = ValidateGeneratedMode(request, hasValidPaymentType, paymentType, errors);
        }

        if (createMode == InstallmentCreateMode.Explicit)
        {
            expenseTotal = ValidateExplicitMode(request, errors);
        }

        ValidateShares(request.Shares, expenseTotal, errors);
        return errors;
    }

    private static decimal? ValidateGeneratedMode(
        CreateExpenseRequest request,
        bool hasValidPaymentType,
        ExpensePaymentType? paymentType,
        List<AppError> errors)
    {
        if (request.TotalAmount is null)
        {
            errors.Add(AppError.Validation("expense.total_amount.required", "TotalAmount is required."));
        }
        else
        {
            if (request.TotalAmount <= 0)
            {
                errors.Add(AppError.Validation("expense.total_amount.invalid", "TotalAmount must be greater than zero."));
            }

            if (!Money.HasValidScale(request.TotalAmount.Value))
            {
                errors.Add(AppError.Validation("expense.total_amount.scale", "TotalAmount must have at most 2 decimal places."));
            }
        }

        if (request.Installments is { Count: > 0 })
        {
            errors.Add(AppError.Validation(
                "expense.installments.unsupported",
                "Installments must not be provided when InstallmentCreateMode is Generated."));
        }

        if (hasValidPaymentType
            && !Expense.TryResolveInstallmentsQuantity(paymentType!.Value, request.TotalInstallments, out _))
        {
            errors.Add(AppError.Validation(
                "expense.total_installments.invalid",
                "TotalInstallments must be 1 for cash or greater than 1 for installment."));
        }

        return request.TotalAmount is decimal totalAmount && totalAmount > 0m && Money.HasValidScale(totalAmount)
            ? totalAmount
            : null;
    }

    private static decimal? ValidateExplicitMode(CreateExpenseRequest request, List<AppError> errors)
    {
        if (request.TotalAmount is not null)
        {
            errors.Add(AppError.Validation(
                "expense.total_amount.unsupported",
                "TotalAmount must not be provided when InstallmentCreateMode is Explicit."));
        }

        if (request.TotalInstallments is not null)
        {
            errors.Add(AppError.Validation(
                "expense.total_installments.unsupported",
                "TotalInstallments must not be provided when InstallmentCreateMode is Explicit."));
        }

        if (request.FirstDueDate is not null)
        {
            errors.Add(AppError.Validation(
                "expense.first_due_date.unsupported",
                "FirstDueDate must not be provided when InstallmentCreateMode is Explicit."));
        }

        if (request.Installments is not { Count: > 0 })
        {
            errors.Add(AppError.Validation(
                "expense.installments.required",
                "Installments are required when InstallmentCreateMode is Explicit."));
            return null;
        }

        var canComputeTotal = true;
        var totalAmount = 0m;

        for (var index = 0; index < request.Installments.Count; index++)
        {
            var installment = request.Installments.ElementAt(index);
            var pathIndex = index + 1;

            if (installment.Amount is null)
            {
                errors.Add(AppError.Validation(
                    $"expense.installments[{pathIndex}].amount.required",
                    $"Installments[{pathIndex}].Amount is required."));
                canComputeTotal = false;
            }
            else
            {
                if (installment.Amount <= 0)
                {
                    errors.Add(AppError.Validation(
                        $"expense.installments[{pathIndex}].amount.invalid",
                        $"Installments[{pathIndex}].Amount must be greater than zero."));
                    canComputeTotal = false;
                }

                if (!Money.HasValidScale(installment.Amount.Value))
                {
                    errors.Add(AppError.Validation(
                        $"expense.installments[{pathIndex}].amount.scale",
                        $"Installments[{pathIndex}].Amount must have at most 2 decimal places."));
                    canComputeTotal = false;
                }

                if (installment.Amount > 0 && Money.HasValidScale(installment.Amount.Value))
                {
                    totalAmount += installment.Amount.Value;
                }
            }

            if (installment.DueDate is null)
            {
                errors.Add(AppError.Validation(
                    $"expense.installments[{pathIndex}].due_date.required",
                    $"Installments[{pathIndex}].DueDate is required."));
            }
        }

        return canComputeTotal ? totalAmount : null;
    }

    private static void ValidateShares(
        IReadOnlyCollection<CreateExpenseShareRequest>? shares,
        decimal? expenseTotal,
        List<AppError> errors)
    {
        if (shares == null)
        {
            return;
        }

        var shareList = shares.ToList();
        var totalSharedAmount = 0m;
        var canCompareTotals = expenseTotal is not null;

        for (var shareIndex = 0; shareIndex < shareList.Count; shareIndex++)
        {
            var share = shareList[shareIndex];
            var sharePathIndex = shareIndex + 1;

            if (share.PersonId is null)
            {
                errors.Add(AppError.Validation(
                    $"expense.shares[{sharePathIndex}].person_id.required",
                    $"Shares[{sharePathIndex}].PersonId is required."));
            }
            else if (share.PersonId == Guid.Empty)
            {
                errors.Add(AppError.Validation(
                    $"expense.shares[{sharePathIndex}].person_id.invalid",
                    $"Shares[{sharePathIndex}].PersonId must be a valid identifier."));
            }

            var shareTotal = ExpenseShareInstallmentRequestValidation.ValidateInstallments(
                share.Installments,
                $"expense.shares[{sharePathIndex}].installments",
                $"Shares[{sharePathIndex}].Installments",
                installment => installment.Amount,
                installment => installment.DueDate,
                errors);

            if (shareTotal is not null)
            {
                totalSharedAmount += shareTotal.Value;
            }
            else
            {
                canCompareTotals = false;
            }
        }

        if (canCompareTotals && totalSharedAmount > expenseTotal!.Value)
        {
            errors.Add(AppError.Validation(
                "expense.shares.total.exceeded",
                "The total shared amount must be less than or equal to the expense total amount."));
        }
    }
}
