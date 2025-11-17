using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;
using Budgets.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Budgets.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly BudgetDbContext _context;
    private readonly DbSet<Budget> _budgets;

    public BudgetRepository(BudgetDbContext context)
    {
        _context = context;
        _budgets = context.Budgets;
    }
    
    public async Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _budgets
            .Include(b => b.BudgetCategories)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Budget>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _budgets
            .Include(b => b.BudgetCategories)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Budget>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _budgets
            .Include(b => b.BudgetCategories)
            .Where(b => b.PeriodStart >= startDate && b.PeriodEnd <= endDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        await _budgets.AddAsync(budget, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        _budgets.Update(budget);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = await GetByIdAsync(id, cancellationToken);
        if (budget != null)
        {
            _budgets.Remove(budget);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _budgets.AnyAsync(b => b.Id == id, cancellationToken);
    }
}