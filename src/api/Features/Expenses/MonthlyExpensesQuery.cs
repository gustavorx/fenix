using api.Data;
using api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace api.Features.Expenses;

public class MonthlyExpensesQuery(FenixContext context)
{
    public async Task<MonthlyExpensesResponse> ExecuteAsync(int month, int year, CancellationToken cancellationToken)
    {
        var installments = await context.Installments
            .AsNoTracking()
            .Include(installment => installment.Expense)
            .Where(installment => installment.DueDate.Month == month && installment.DueDate.Year == year)
            .OrderBy(installment => installment.DueDate)
            .ThenBy(installment => installment.Number)
            .ToListAsync(cancellationToken);

        return new MonthlyExpensesResponse
        {
            Month = month,
            Year = year,
            TotalAmount = installments.Sum(installment => installment.Amount),
            Installments = installments.Select(installment => new MonthlyExpenseInstallmentResponse
            {
                InstallmentId = installment.Id,
                ExpenseId = installment.ExpenseId,
                Description = installment.Expense.Description,
                PaymentType = installment.Expense.Type,
                TotalAmount = installment.Expense.TotalAmount,
                TotalInstallments = installment.Expense.InstallmentsQuantity ?? 1,
                InstallmentNumber = installment.Number,
                InstallmentAmount = installment.Amount,
                PurchaseDate = installment.Expense.Date,
                DueDate = installment.DueDate,
                Paid = installment.Paid
            }).ToList()
        };
    }
}
