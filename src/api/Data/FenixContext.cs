using api.Entities;
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
    public DbSet<Card> Cards { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FenixContext).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}
