using api.Auth;
using api.Data;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;

public class GetMonthlyExpenseShareInstallmentsUseCase(FenixContext context, ICurrentUser currentUser)
{
    public async Task<Result<MonthlyExpenseShareInstallmentsResponse>> ExecuteAsync(
        int month,
        int year,
        CancellationToken cancellationToken)
    {
        if (month is < 1 or > 12)
        {
            return Result<MonthlyExpenseShareInstallmentsResponse>.Failure(
                AppError.Validation(
                    "expense_share_installment.month.invalid",
                    "Month must be between 1 and 12."));
        }

        if (year is < 1 or > 9999)
        {
            return Result<MonthlyExpenseShareInstallmentsResponse>.Failure(
                AppError.Validation(
                    "expense_share_installment.year.invalid",
                    "Year must be between 1 and 9999."));
        }

        var installments = await context.ExpenseShareInstallments
            .AsNoTracking()
            .Include(installment => installment.ExpenseShare)
                .ThenInclude(share => share.Expense)
            .Include(installment => installment.ExpenseShare)
                .ThenInclude(share => share.Person)
            .Where(installment =>
                installment.ExpenseShare.Expense.UserId == currentUser.UserId &&
                installment.DueDate.Month == month &&
                installment.DueDate.Year == year)
            .OrderBy(installment => installment.DueDate)
            .ThenBy(installment => installment.ExpenseShare.Expense.Description)
            .ThenBy(installment => installment.ExpenseShare.Person == null
                ? null
                : installment.ExpenseShare.Person.Name)
            .ThenBy(installment => installment.Id)
            .ToListAsync(cancellationToken);

        return Result<MonthlyExpenseShareInstallmentsResponse>.Success(
            new MonthlyExpenseShareInstallmentsResponse
            {
                Month = month,
                Year = year,
                TotalAmount = installments
                    .Aggregate(Money.Zero, (total, installment) => total + installment.Amount)
                    .Value,
                Items = installments.Select(ToResponse).ToList()
            });
    }

    private static MonthlyExpenseShareInstallmentResponse ToResponse(Entities.ExpenseShareInstallment installment)
    {
        return new MonthlyExpenseShareInstallmentResponse
        {
            ShareInstallmentId = installment.Id,
            ShareId = installment.ExpenseShareId,
            ExpenseId = installment.ExpenseShare.ExpenseId,
            ExpenseDescription = installment.ExpenseShare.Expense.Description,
            PersonId = installment.ExpenseShare.PersonId,
            PersonName = installment.ExpenseShare.Person?.Name,
            Amount = installment.Amount.Value,
            DueDate = installment.DueDate,
            PaidDate = installment.PaidDate,
            IsPaid = installment.IsPaid
        };
    }
}
