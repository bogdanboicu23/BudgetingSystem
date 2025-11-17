using Budgets.Domain.Entities;

namespace Budgets.Domain.Repositories;

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Budget>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Budget>> GetByPeriodAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task AddAsync(Budget budget, CancellationToken cancellationToken = default);
    Task UpdateAsync(Budget budget, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}