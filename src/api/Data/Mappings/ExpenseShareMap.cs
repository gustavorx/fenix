using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Data.Mappings;

public class ExpenseShareMap : IEntityTypeConfiguration<ExpenseShare>
{
    public void Configure(EntityTypeBuilder<ExpenseShare> builder)
    {
        builder.ToTable("ExpenseShares");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Amount)
            .IsRequired();

        builder.HasOne(s => s.Expense)
            .WithMany(e => e.Shares)
            .HasForeignKey(s => s.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Person)
            .WithMany(p => p.Shares)
            .HasForeignKey(s => s.PersonId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(s => s.Installments)
            .WithOne(i => i.ExpenseShare)
            .HasForeignKey(i => i.ExpenseShareId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
