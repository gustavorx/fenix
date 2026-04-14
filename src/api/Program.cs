using System.Diagnostics;
using api.Auth;
using api.Data;
using api.Features.Expenses.CreateExpense;
using api.Features.Expenses.DeleteExpense;
using api.Features.Expenses.GetAllExpenses;
using api.Features.Expenses.GetExpenseById;
using api.Features.Expenses.GetMonthlyExpenses;
using api.Features.Incomes.CreateIncome;
using api.Features.Incomes.DeleteIncome;
using api.Features.Incomes.GetAllIncomes;
using api.Features.Incomes.GetIncomeById;
using api.Features.Incomes.GetMonthlyIncomes;
using api.Features.Incomes.UpdateIncome;
using api.Observability;
using api.Shared;
using Microsoft.EntityFrameworkCore;

Activity.DefaultIdFormat = ActivityIdFormat.W3C;
Activity.ForceDefaultIdFormat = true;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.Configure(options =>
{
    options.ActivityTrackingOptions =
        ActivityTrackingOptions.TraceId |
        ActivityTrackingOptions.SpanId |
        ActivityTrackingOptions.ParentId;
});

builder.Services.AddDbContext<FenixContext>((serviceProvider, options) =>
    options
        .UseNpgsql(builder.Configuration.GetConnectionString(("DefaultConnection")))
        .AddInterceptors(serviceProvider.GetRequiredService<DatabaseCommandMetricsInterceptor>()));

builder.Services.AddScoped<ICurrentUser, DevelopmentCurrentUser>();

builder.Services.AddScoped<CreateExpenseUseCase>();
builder.Services.AddScoped<DeleteExpenseUseCase>();
builder.Services.AddScoped<IValidator<CreateExpenseRequest>, CreateExpenseRequestValidator>();
builder.Services.AddScoped<GetAllExpensesUseCase>();
builder.Services.AddScoped<GetExpenseByIdUseCase>();
builder.Services.AddScoped<GetMonthlyExpensesUseCase>();

builder.Services.AddScoped<CreateIncomeUseCase>();
builder.Services.AddScoped<DeleteIncomeUseCase>();
builder.Services.AddScoped<IValidator<CreateIncomeRequest>, CreateIncomeRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateIncomeRequest>, UpdateIncomeRequestValidator>();
builder.Services.AddScoped<GetAllIncomesUseCase>();
builder.Services.AddScoped<GetIncomeByIdUseCase>();
builder.Services.AddScoped<GetMonthlyIncomesUseCase>();
builder.Services.AddScoped<UpdateIncomeUseCase>();

builder.Services.AddControllers();
builder.Services.AddFenixObservability(builder.Configuration);

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
