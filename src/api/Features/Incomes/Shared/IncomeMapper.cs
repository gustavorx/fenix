using api.Entities;

namespace api.Features.Incomes.Shared;

public static class IncomeMapper
{
    public static IncomeResponse ToResponse(this Income income)
    {
        return new IncomeResponse
        {
            Id = income.Id,
            Description = income.Description,
            Amount = income.Amount.Value,
            Date = income.Date
        };
    }
}