using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Budgets.Infrastructure.Data.Configurations;

public class BudgetCategoryConfiguration : IEntityTypeConfiguration<BudgetCategory>
{
    public void Configure(EntityTypeBuilder<BudgetCategory> builder)
    {
        builder.ToTable("budget_categories");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Id)
            .HasColumnName("id")
            .IsRequired();

        builder.Property(bc => bc.Name)
            .HasColumnName("name")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(bc => bc.PlannedAmount)
            .HasColumnName("planned_amount")
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(bc => bc.SpentAmount)
            .HasColumnName("spent_amount")
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(bc => bc.CreatedDate)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(bc => bc.ModifiedDate)
            .HasColumnName("updated_at");

        builder.Property<Guid>("BudgetId")
            .HasColumnName("budget_id")
            .IsRequired();

        builder.Ignore(bc => bc.RemainingAmount);
    }
}