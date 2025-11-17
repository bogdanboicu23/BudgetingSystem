using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Budgets.Infrastructure.Data.Configurations;

public class BudgetConfiguration : IEntityTypeConfiguration<Budget>
{
    public void Configure(EntityTypeBuilder<Budget> builder)
    {
        builder.ToTable("budgets");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(b => b.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(b => b.PeriodStart)
            .HasColumnName("period_start")
            .IsRequired();

        builder.Property(b => b.PeriodEnd)
            .HasColumnName("period_end")
            .IsRequired();

        builder.Property(b => b.CreatedDate)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(b => b.ModifiedDate)
            .HasColumnName("updated_at");

        builder.HasMany(b => b.BudgetCategories)
            .WithOne()
            .HasForeignKey("BudgetId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(b => b.BudgetCategories)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // Ignore computed properties
        builder.Ignore(b => b.Amount);
        builder.Ignore(b => b.SpentAmount);
        builder.Ignore(b => b.PlannedIncomeAmount);
        builder.Ignore(b => b.ActualIncomeAmount);
        builder.Ignore(b => b.PlannedIncomes);
        builder.Ignore(b => b.PlannedExpenses);
    }
}