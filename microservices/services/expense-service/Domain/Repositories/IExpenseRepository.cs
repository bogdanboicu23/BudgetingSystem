using ExpenseService.Domain.Entities;
using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Domain.Repositories;

public interface IExpenseRepository
{
    Task<Expense?> GetByIdAsync(ExpenseId id);
    Task<IEnumerable<Expense>> GetByUserIdAsync(UserId userId);
    Task<IEnumerable<Expense>> GetByBudgetIdAsync(BudgetId budgetId);
    Task<IEnumerable<Expense>> GetByCategoryAsync(ExpenseCategory category);
    Task<IEnumerable<Expense>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<Expense> AddAsync(Expense expense);
    Task UpdateAsync(Expense expense);
    Task DeleteAsync(ExpenseId id);
}