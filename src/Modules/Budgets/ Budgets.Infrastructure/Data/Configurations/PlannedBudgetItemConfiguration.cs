using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Budgets.Infrastructure.Data.Configurations;

public class PlannedBudgetItemConfiguration : IEntityTypeConfiguration<PlannedBudgetItem>
{
    public void Configure(EntityTypeBuilder<PlannedBudgetItem> builder)
    {
        builder.ToTable("PlannedBudgetItems");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.PlannedAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.LinkedAmount)
            .HasColumnType("decimal(18,2)")
            .HasDefaultValue(0);

        builder.Property(e => e.Category)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(e => e.Type)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(e => e.DueDate);

        builder.Property(e => e.IsRecurring)
            .HasDefaultValue(false);

        builder.Property(e => e.Frequency)
            .HasConversion<string>();

        // Ignore computed properties
        builder.Ignore(e => e.RemainingAmount);
        builder.Ignore(e => e.LinkedTransactionIds);

        // Shadow property for BudgetId foreign key
        builder.Property<Guid>("BudgetId")
            .IsRequired();

        builder.HasIndex("BudgetId");
    }
}