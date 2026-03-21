using api.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<FenixContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString(("DefaultConnection"))));

builder.Services.AddControllers();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var context = scope.ServiceProvider.GetRequiredService<FenixContext>();
    await AppDataInitializer.InitializeAsync(context);
}

app.MapControllers();

app.Run();
