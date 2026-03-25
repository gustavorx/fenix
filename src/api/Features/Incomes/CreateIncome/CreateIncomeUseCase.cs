using api.Data;
using api.Entities;
using api.Features.Incomes.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Incomes.CreateIncome;

public class CreateIncomeUseCase(FenixContext context)
{
    public async Task<Result<IncomeResponse>> ExecuteAsync(CreateIncomeRequest request, CancellationToken cancellationToken)
    {
        var errors = new List<AppError>();

        if (string.IsNullOrWhiteSpace(request.Description))
        {
            errors.Add(AppError.Validation("income.description.required", "Description is required."));
        }

        if (request.Amount <= 0)
        {
            errors.Add(AppError.Validation("income.amount.invalid", "Amount must be greater than zero."));
        }

        if (!Money.HasValidScale(request.Amount))
        {
            errors.Add(AppError.Validation("income.amount.scale", "Amount must have at most 2 decimal places."));
        }

        if (errors.Count > 0)
        {
            return Result<IncomeResponse>.Failure(errors);
        }

        var income = new Income
        {
            Id = Guid.NewGuid(),
            Description = request.Description.Trim(),
            Amount = Money.Create(request.Amount),
            Date = request.Date.Normalize(),
            UserId = AppDataInitializer.DefaultUserId
        };

        context.Incomes.Add(income);
        await context.SaveChangesAsync(cancellationToken);

        return Result<IncomeResponse>.Success(income.ToResponse());
    }
}
