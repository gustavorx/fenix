using api.Data;
using api.DTOs;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/expenses")]
public class ExpenseController(FenixContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest? request)
    {
        if (request == null)
        {
            return BadRequest("Invalid data.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Description is required.");
        }

        if (request.TotalAmount <= 0)
        {
            return BadRequest("TotalAmount must be greater than zero.");
        }

        var normalizedPaymentType = NormalizePaymentType(request.PaymentType);
        if (normalizedPaymentType == null)
        {
            return BadRequest("PaymentType must be 'cash' or 'installment'.");
        }

        var expense = new Expense
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            TotalAmount = request.TotalAmount,
            Date = NormalizeDate(request.Date),
            Type = normalizedPaymentType,
            InstallmentsQuantity = normalizedPaymentType == "Installment" ? request.InstallmentsQuantity : null,
            UserId = AppDataInitializer.DefaultUserId,
            Installments = []
        };

        if (normalizedPaymentType == "Installment")
        {
            var installmentsQuantity = request.InstallmentsQuantity.GetValueOrDefault();
            if (installmentsQuantity < 2)
            {
                return BadRequest("Installment expenses must have at least 2 installments.");
            }

            var firstDueDate = NormalizeDate(request.FirstDueDate ?? request.Date ?? DateTime.UtcNow);
            var installmentAmounts = SplitAmount(request.TotalAmount, installmentsQuantity);

            expense.InstallmentsQuantity = installmentsQuantity;
            expense.Installments = installmentAmounts
                .Select((amount, index) => new Installment
                {
                    Id = Guid.NewGuid(),
                    Number = index + 1,
                    Amount = amount,
                    DueDate = firstDueDate.AddMonths(index),
                    Paid = false,
                    ExpenseId = expense.Id
                })
                .ToList();
        }

        context.Expenses.Add(expense);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetExpenseById), new { id = expense.Id }, Map(expense));
    }

    [HttpGet]
    public async Task<IActionResult> GetExpenses()
    {
        var expenses = await context.Expenses
            .AsNoTracking()
            .Include(expense => expense.Installments)
            .OrderByDescending(expense => expense.Date)
            .ToListAsync();

        return Ok(expenses.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetExpenseById(Guid id)
    {
        var expense = await context.Expenses
            .AsNoTracking()
            .Include(item => item.Installments)
            .FirstOrDefaultAsync(item => item.Id == id);

        if (expense == null)
        {
            return NotFound();
        }

        return Ok(Map(expense));
    }

    private static ExpenseResponse Map(Expense expense)
    {
        var orderedInstallments = expense.Installments?
            .OrderBy(installment => installment.Number)
            .ToList() ?? [];

        return new ExpenseResponse
        {
            Id = expense.Id,
            Description = expense.Description,
            TotalAmount = expense.TotalAmount,
            Date = expense.Date,
            PaymentType = expense.Type,
            InstallmentsQuantity = expense.InstallmentsQuantity,
            CurrentInstallmentNumber = GetCurrentInstallmentNumber(orderedInstallments),
            Installments = orderedInstallments.Select(installment => new InstallmentResponse
            {
                Id = installment.Id,
                Number = installment.Number,
                Amount = installment.Amount,
                DueDate = installment.DueDate,
                Paid = installment.Paid
            }).ToList()
        };
    }

    private static int? GetCurrentInstallmentNumber(IReadOnlyList<Installment> installments)
    {
        if (installments.Count == 0)
        {
            return null;
        }

        var currentInstallment = installments.FirstOrDefault(installment => !installment.Paid);
        return currentInstallment?.Number ?? installments[^1].Number;
    }

    private static string? NormalizePaymentType(string paymentType)
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

    private static IReadOnlyList<decimal> SplitAmount(decimal totalAmount, int installmentsQuantity)
    {
        var baseAmount = Math.Floor((totalAmount / installmentsQuantity) * 100) / 100;
        var amounts = Enumerable.Repeat(baseAmount, installmentsQuantity).ToArray();
        var allocated = baseAmount * installmentsQuantity;
        var remainder = totalAmount - allocated;

        for (var index = 0; remainder > 0; index = (index + 1) % installmentsQuantity)
        {
            amounts[index] += 0.01m;
            remainder -= 0.01m;
        }

        return amounts;
    }

    private static DateTime NormalizeDate(DateTime? date)
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
