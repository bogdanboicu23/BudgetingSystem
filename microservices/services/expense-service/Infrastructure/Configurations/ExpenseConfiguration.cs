using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenseService.Domain.Entities;
using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Infrastructure.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .HasConversion(
                v => v.Value,
                v => ExpenseId.From(v))
            .IsRequired();

        builder.Property(e => e.UserId)
            .HasConversion(
                v => v.Value,
                v => UserId.From(v))
            .IsRequired();

        builder.Property(e => e.BudgetId)
            .HasConversion(
                v => v != null ? v.Value : (Guid?)null,
                v => v.HasValue ? BudgetId.From(v.Value) : null);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.OwnsOne(e => e.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(e => e.Category)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(e => e.Date)
            .IsRequired();

        builder.Property(e => e.Merchant)
            .HasMaxLength(200);

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.LastModifiedAt);

        builder.Ignore(e => e.DomainEvents);

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Expenses_UserId");

        builder.HasIndex(e => e.BudgetId)
            .HasDatabaseName("IX_Expenses_BudgetId");

        builder.HasIndex(e => e.Date)
            .HasDatabaseName("IX_Expenses_Date");
    }
}