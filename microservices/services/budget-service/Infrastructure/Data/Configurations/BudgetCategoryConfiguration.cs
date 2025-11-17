using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BudgetService.Domain.Entities;

namespace BudgetService.Infrastructure.Data.Configurations;

public class BudgetCategoryConfiguration : IEntityTypeConfiguration<BudgetCategory>
{
    public void Configure(EntityTypeBuilder<BudgetCategory> builder)
    {
        builder.ToTable("BudgetCategories");

        builder.HasKey("BudgetId", "Name");

        builder.Property("BudgetId")
            .IsRequired();

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.OwnsOne(c => c.AllocatedAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("AllocatedAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3);
        });

        builder.OwnsOne(c => c.SpentAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("SpentAmount")
                .HasPrecision(18, 2);

            money.Property(m => m.Currency)
                .HasColumnName("SpentCurrency")
                .HasMaxLength(3);
        });

        builder.Ignore(c => c.RemainingAmount);
        builder.Ignore(c => c.UtilizationPercentage);
        builder.Ignore(c => c.IsOverBudget);
    }
}