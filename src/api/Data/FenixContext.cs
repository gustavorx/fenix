using api.Data.Converters;
using api.Entities;
using api.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace api.Data;

public class FenixContext(DbContextOptions<FenixContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Income> Incomes { get; set; } = null!;
    public DbSet<Expense> Expenses { get; set; } = null!;
    public DbSet<Installment> Installments { get; set; } = null!;
    public DbSet<Person> People { get; set; } = null!;
    public DbSet<ExpenseShare> ExpenseShares { get; set; } = null!;
    public DbSet<ExpenseShareInstallment> ExpenseShareInstallments { get; set; } = null!;
    public DbSet<Card> Cards { get; set; } = null!;

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<Money>()
            .HaveConversion<MoneyValueConverter>()
            .HaveColumnType("decimal(18,2)");

        configurationBuilder.Properties<Money?>()
            .HaveConversion<NullableMoneyValueConverter>()
            .HaveColumnType("decimal(18,2)");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FenixContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
