using BudgetService.Domain.Entities;
using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(BudgetId id);
    Task<IEnumerable<Budget>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Budget>> GetActiveBudgetsByUserIdAsync(UserId userId);
    Task<Budget?> GetActiveBudgetForPeriodAsync(UserId userId, DateTime date);
    Task AddAsync(Budget budget);
    Task UpdateAsync(Budget budget);
    Task DeleteAsync(BudgetId id);
    Task<bool> ExistsAsync(BudgetId id);
    Task<int> CountByUserIdAsync(UserId userId);
}