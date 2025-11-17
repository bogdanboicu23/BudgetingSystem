using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;

namespace Budgets.Infrastructure.Repositories;

public class InMemoryBudgetRepository : IBudgetRepository
{
    private readonly List<Budget> _budgets = new();

    public InMemoryBudgetRepository()
    {
        SeedInitialData();
    }

    public Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var budget = _budgets.FirstOrDefault(b => b.Id == id);
        return Task.FromResult(budget);
    }

    public Task<IEnumerable<Budget>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<Budget>>(_budgets.ToList());
    }

    public Task<IEnumerable<Budget>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        var filteredBudgets = _budgets.Where(b =>
            b.PeriodStart >= startDate && b.PeriodEnd <= endDate).ToList();
        return Task.FromResult<IEnumerable<Budget>>(filteredBudgets);
    }

    public Task AddAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        _budgets.Add(budget);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default)
    {
        var existingIndex = _budgets.FindIndex(b => b.Id == budget.Id);
        if (existingIndex >= 0)
        {
            _budgets[existingIndex] = budget;
        }
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _budgets.RemoveAll(b => b.Id == id);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_budgets.Any(b => b.Id == id));
    }

    private void SeedInitialData()
    {
        var budget1 = new Budget(Guid.NewGuid(), "November 2024 Budget",
            new DateTime(2024, 11, 1), new DateTime(2024, 11, 30));
        budget1.AddCategory("Housing", 1200m);
        budget1.AddCategory("Food", 400m);
        budget1.AddCategory("Transportation", 300m);
        budget1.AddCategory("Entertainment", 200m);
        budget1.AddCategory("Utilities", 150m);

        var budget2 = new Budget(Guid.NewGuid(), "Weekly Groceries Budget",
            new DateTime(2024, 11, 11), new DateTime(2024, 11, 17));
        budget2.AddCategory("Groceries", 100m);
        budget2.AddCategory("Household Items", 50m);

        var budget3 = new Budget(Guid.NewGuid(), "Holiday Savings",
            new DateTime(2024, 11, 1), new DateTime(2024, 12, 25));
        budget3.AddCategory("Gifts", 800m);
        budget3.AddCategory("Travel", 500m);
        budget3.AddCategory("Food & Entertainment", 300m);

        _budgets.AddRange(new[] { budget1, budget2, budget3 });
    }
}