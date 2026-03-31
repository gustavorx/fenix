using api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace api.Data.Mappings;

public class ExpenseMap : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.TotalAmount)
            .IsRequired();

        builder.Property(e => e.PurchaseDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.PaymentType)
            .IsRequired();

        builder.Property(e => e.InstallmentsQuantity)
            .IsRequired();

        builder.HasOne(e => e.User)
            .WithMany(u => u.Expenses)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Card)
            .WithMany(c => c.Expenses)
            .HasForeignKey(e => e.CardId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Installments)
            .WithOne(i => i.Expense)
            .HasForeignKey(i => i.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Shares)
            .WithOne(s => s.Expense)
            .HasForeignKey(s => s.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
