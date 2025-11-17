using Budgets.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Budgets.Infrastructure.Data;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options)
    {
    }

    public DbSet<Budget> Budgets { get; set; }
    public DbSet<BudgetCategory> BudgetCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BudgetDbContext).Assembly);
    }
}