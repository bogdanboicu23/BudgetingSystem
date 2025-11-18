using ExpenseService.Domain.Entities;
using ExpenseService.Domain.Repositories;
using ExpenseService.Domain.ValueObjects;
using ExpenseService.Infrastructure.Data;

namespace ExpenseService.Infrastructure.Repositories;

public class ExpenseRepository : IExpenseRepository
{
    private readonly ExpenseDbContext _context;

    public ExpenseRepository(ExpenseDbContext context)
    {
        _context = context;
    }

    public async Task<Expense?> GetByIdAsync(ExpenseId id)
    {
        return await _context.Expenses
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<IEnumerable<Expense>> GetByUserIdAsync(UserId userId)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByBudgetIdAsync(BudgetId budgetId)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.BudgetId == budgetId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.Category == category)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.Expenses
            .AsNoTracking()
            .Where(e => e.Date >= startDate && e.Date <= endDate)
            .OrderByDescending(e => e.Date)
            .ToListAsync();
    }

    public async Task<Expense> AddAsync(Expense expense)
    {
        var result = await _context.Expenses.AddAsync(expense);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task UpdateAsync(Expense expense)
    {
        _context.Expenses.Update(expense);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(ExpenseId id)
    {
        var expense = await _context.Expenses.FindAsync(id);
        if (expense != null)
        {
            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
        }
    }
}