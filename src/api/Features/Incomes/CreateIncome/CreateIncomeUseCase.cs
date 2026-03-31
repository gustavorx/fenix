using api.Data;
using api.Entities;
using api.Features.Incomes.Shared;
using api.Shared;
using api.ValueObjects;

namespace api.Features.Incomes.CreateIncome;

public class CreateIncomeUseCase(FenixContext context, IValidator<CreateIncomeRequest> validator)
{
    public async Task<Result<IncomeResponse>> ExecuteAsync(CreateIncomeRequest request, CancellationToken cancellationToken)
    {
        var errors = validator.Validate(request);
        if (errors.Count > 0)
        {
            return Result<IncomeResponse>.Failure(errors);
        }

        var income = Income.Create(
            request.Description!,
            Money.Create(request.Amount),
            request.ReceivedDate!.Value,
            AppDataInitializer.DefaultUserId);

        context.Incomes.Add(income);
        await context.SaveChangesAsync(cancellationToken);

        return Result<IncomeResponse>.Success(income.ToResponse());
    }
}
