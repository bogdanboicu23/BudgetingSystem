using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetService.Domain.Entities;
using BudgetService.Domain.ValueObjects;

namespace BudgetService.Infrastructure.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("Budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasConversion(
                id => id.Value,
                value => BudgetId.From(value))
            .IsRequired();

        builder.Property(b => b.UserId)
            .HasConversion(
                id => id.Value,
                value => UserId.From(value))
            .IsRequired();

        builder.Property(b => b.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.OwnsOne(b => b.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(b => b.SpentAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("SpentAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("SpentCurrency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(b => b.Period, period =>
        {
            period.Property(p => p.StartDate)
                .HasColumnName("StartDate");

            period.Property(p => p.EndDate)
                .HasColumnName("EndDate");

            period.Property(p => p.PeriodType)
                .HasColumnName("PeriodType")
                .HasConversion<string>();
        });

        builder.Property(b => b.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.LastModifiedAt);

        builder.HasMany(b => b.Categories)
            .WithOne()
            .HasForeignKey("BudgetId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(b => b.DomainEvents);

        builder.HasIndex(b => b.UserId);
        builder.HasIndex(b => new { b.UserId, b.Status });
    }
}