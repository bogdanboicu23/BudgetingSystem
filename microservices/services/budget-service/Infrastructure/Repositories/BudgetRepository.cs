using Microsoft.EntityFrameworkCore;
using BudgetService.Domain.Entities;
using BudgetService.Domain.Repositories;
using BudgetService.Domain.ValueObjects;
using BudgetService.Infrastructure.Data;

namespace BudgetService.Infrastructure.Repositories;

public class BudgetRepository : IBudgetRepository
{
    private readonly BudgetDbContext _context;

    public BudgetRepository(BudgetDbContext context)
    {
        _context = context;
    }

    public async Task<Budget?> GetByIdAsync(BudgetId id)
    {
        return await _context.Budgets
            .Include(b => b.Categories)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<IEnumerable<Budget>> GetByUserIdAsync(UserId userId)
    {
        return await _context.Budgets
            .Include(b => b.Categories)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Budget>> GetActiveBudgetsByUserIdAsync(UserId userId)
    {
        return await _context.Budgets
            .Include(b => b.Categories)
            .Where(b => b.UserId == userId && b.Status == BudgetStatus.Active)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<Budget?> GetActiveBudgetForPeriodAsync(UserId userId, DateTime date)
    {
        return await _context.Budgets
            .Include(b => b.Categories)
            .Where(b => b.UserId == userId
                && b.Status == BudgetStatus.Active
                && b.Period.StartDate <= date
                && b.Period.EndDate >= date)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(Budget budget)
    {
        await _context.Budgets.AddAsync(budget);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Budget budget)
    {
        _context.Budgets.Update(budget);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(BudgetId id)
    {
        var budget = await _context.Budgets.FindAsync(id.Value);
        if (budget != null)
        {
            _context.Budgets.Remove(budget);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(BudgetId id)
    {
        return await _context.Budgets
            .AnyAsync(b => b.Id == id);
    }

    public async Task<int> CountByUserIdAsync(UserId userId)
    {
        return await _context.Budgets
            .CountAsync(b => b.UserId == userId);
    }
}