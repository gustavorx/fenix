using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Data.Mappings;

public class InstallmentMap : IEntityTypeConfiguration<Installment>
{
    public void Configure(EntityTypeBuilder<Installment> builder)
    {
        builder.ToTable("Installments");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Number)
            .IsRequired();

        builder.Property(i => i.Amount)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.Property(i => i.Paid)
            .IsRequired();

        builder.HasOne(i => i.Expense)
            .WithMany(e => e.Installments)
            .HasForeignKey(i => i.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
