using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Budgets.Infrastructure.Data.Configurations;

public class BudgetPerformanceSummaryConfiguration : IEntityTypeConfiguration<BudgetPerformanceSummary>
{
    public void Configure(EntityTypeBuilder<BudgetPerformanceSummary> builder)
    {
        builder.ToTable("BudgetPerformanceSummaries");

        // Since BudgetPerformanceSummary doesn't inherit from BaseEntity, we need to add Id manually
        builder.Property<Guid>("Id")
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.HasKey("Id");

        builder.Property(e => e.TimeProgress)
            .IsRequired();

        builder.Property(e => e.SpendingProgress)
            .IsRequired();

        builder.Property(e => e.IncomeProgress)
            .IsRequired();

        builder.Property(e => e.RemainingBudget)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.IncomeVariance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.ExpenseVariance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>();

        // Ignore computed properties
        builder.Ignore(e => e.IsOnTrack);
        builder.Ignore(e => e.IsOverspending);
        builder.Ignore(e => e.IsUnderspending);

        // Add timestamp for tracking
        builder.Property<DateTime>("CreatedAt")
            .IsRequired()
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Shadow property for BudgetId foreign key
        builder.Property<Guid>("BudgetId")
            .IsRequired();

        builder.HasIndex("BudgetId");
        builder.HasIndex("CreatedAt");
    }
}