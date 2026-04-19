using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Data.Mappings;

public class ExpenseShareInstallmentMap : IEntityTypeConfiguration<ExpenseShareInstallment>
{
    public void Configure(EntityTypeBuilder<ExpenseShareInstallment> builder)
    {
        builder.ToTable("ExpenseShareInstallments");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(i => i.PaidDate)
            .HasColumnType("date")
            .IsRequired(false);

        builder.HasOne(i => i.ExpenseShare)
            .WithMany(s => s.Installments)
            .HasForeignKey(i => i.ExpenseShareId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
