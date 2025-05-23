using Microsoft.EntityFrameworkCore;

namespace api.Database;

public class FenixContext : DbContext
{
    public FenixContext(DbContextOptions<FenixContext> options)
        : base(options)
    {
    }
    public DbSet<Transaction> Transactions { get; set; } = null!;

}
