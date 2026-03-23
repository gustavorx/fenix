using api.Data;
using api.DTOs;
using api.Entities;
using api.ValueObjects;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/incomes")]
public class IncomeController(FenixContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateIncome(
        [FromBody] CreateIncomeRequest? request)
    {
        if (request == null)
        {
            return BadRequest("Invalid data.");
        }

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            return BadRequest("Description is required.");
        }

        if (request.Amount <= 0)
        {
            return BadRequest("Amount must be greater than zero.");
        }

        if (!Money.HasValidScale(request.Amount))
        {
            return BadRequest("Amount must have at most 2 decimal places.");
        }

        var income = new Income
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            Amount = Money.Create(request.Amount),
            Date = NormalizeDate(request.Date),
            UserId = AppDataInitializer.DefaultUserId
        };

        context.Incomes.Add(income);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncomeById), new { id = income.Id }, Map(income));
    }

    [HttpGet]
    public async Task<IActionResult> GetIncomes()
    {
        var incomes = await context.Incomes
            .AsNoTracking()
            .OrderByDescending(income => income.Date)
            .ToListAsync();

        return Ok(incomes.Select(Map));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetIncomeById(Guid id)
    {
        var income = await context.Incomes
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (income == null)
        {
            return NotFound();
        }

        return Ok(Map(income));
    }

    private static IncomeResponse Map(Income income)
    {
        return new IncomeResponse
        {
            Id = income.Id,
            Description = income.Description,
            Amount = income.Amount.Value,
            Date = income.Date
        };
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
