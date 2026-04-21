using api.Auth;
using api.Data;
using api.Shared;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Features.ExpenseShareInstallments.GetMonthlyExpenseShareInstallments;

public class GetMonthlyExpenseShareInstallmentsUseCase(
    FenixContext context,
    ICurrentUser currentUser,
    IValidator<GetMonthlyExpenseShareInstallmentsRequest> validator)
{
    public async Task<Result<MonthlyExpenseShareInstallmentsResponse>> ExecuteAsync(
        GetMonthlyExpenseShareInstallmentsRequest request,
        CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<MonthlyExpenseShareInstallmentsResponse>.Failure(errors);
        }

        var period = MonthPeriod.Create(request.Month, request.Year);

        var installments = await context.ExpenseShareInstallments
            .AsNoTracking()
            .Include(installment => installment.ExpenseShare)
                .ThenInclude(share => share.Expense)
            .Include(installment => installment.ExpenseShare)
                .ThenInclude(share => share.Person)
            .Where(installment =>
                installment.ExpenseShare.Expense.UserId == currentUser.UserId &&
                installment.DueDate >= period.StartDate &&
                installment.DueDate <= period.EndDate)
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
                Month = request.Month,
                Year = request.Year,
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
