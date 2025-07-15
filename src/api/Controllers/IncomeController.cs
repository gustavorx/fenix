using api.Data;
using api.DTOs;
using api.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IncomeController(FenixContext context) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateIncome(
        [FromBody] CreateIncomeRequest? request)
    {
        if (request == null)
            return BadRequest("Invalid data.");
        
        Income income = new();
        income.Id = Guid.NewGuid();
        income.Description = request.Description;
        income.Amount = request.Amount;
        income.Date = DateTime.UtcNow;
        income.UserId = request.UserId;
        
        context.Incomes.Add(income);
        
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetIncomes), new { id = income.Id }, income);
    }
    
    [HttpGet()]
    public async Task<IActionResult> GetIncomes()
    {
        return Ok(await context.Incomes.ToListAsync());
    }
}