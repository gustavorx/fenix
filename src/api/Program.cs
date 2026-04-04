using api.Auth;
using api.Data;
using api.Features.Expenses.CreateExpense;
using api.Features.Expenses.GetAllExpenses;
using api.Features.Expenses.GetExpenseById;
using api.Features.Expenses.GetMonthlyExpenses;
using api.Features.Incomes.CreateIncome;
using api.Features.Incomes.GetAllIncomes;
using api.Features.Incomes.GetIncomeById;
using api.Features.Incomes.GetMonthlyIncomes;
using api.Observability;
using api.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.Configure(options =>
{
    options.ActivityTrackingOptions =
        ActivityTrackingOptions.TraceId |
        ActivityTrackingOptions.SpanId |
        ActivityTrackingOptions.ParentId;
});

builder.Services.AddDbContext<FenixContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(("DefaultConnection"))));

builder.Services.AddScoped<ICurrentUser, DevelopmentCurrentUser>();

builder.Services.AddScoped<CreateExpenseUseCase>();
builder.Services.AddScoped<IValidator<CreateExpenseRequest>, CreateExpenseRequestValidator>();
builder.Services.AddScoped<GetAllExpensesUseCase>();
builder.Services.AddScoped<GetExpenseByIdUseCase>();
builder.Services.AddScoped<GetMonthlyExpensesUseCase>();

builder.Services.AddScoped<CreateIncomeUseCase>();
builder.Services.AddScoped<IValidator<CreateIncomeRequest>, CreateIncomeRequestValidator>();
builder.Services.AddScoped<GetAllIncomesUseCase>();
builder.Services.AddScoped<GetIncomeByIdUseCase>();
builder.Services.AddScoped<GetMonthlyIncomesUseCase>();

builder.Services.AddControllers();
builder.Services.AddFenixObservability();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FenixContext>();
    await AppDataInitializer.InitializeAsync(context);
}

app.UseFenixMetricsEndpoint();
app.UseMiddleware<RequestObservabilityMiddleware>();

app.MapControllers();

app.Run();
