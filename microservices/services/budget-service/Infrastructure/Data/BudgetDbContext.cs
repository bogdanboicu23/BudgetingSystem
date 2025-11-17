using Microsoft.EntityFrameworkCore;
using BudgetService.Domain.Entities;
using BudgetService.Infrastructure.Data.Configurations;

namespace BudgetService.Infrastructure.Data;

public class BudgetDbContext : DbContext
{
    public DbSet<Budget> Budgets { get; set; }

    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new BudgetConfiguration());
        modelBuilder.ApplyConfiguration(new BudgetCategoryConfiguration());
    }
}