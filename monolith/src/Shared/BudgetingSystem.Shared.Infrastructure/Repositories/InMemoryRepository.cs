using BudgetingSystem.Shared.Domain.Entities;
using BudgetingSystem.Shared.Domain.Interfaces;

namespace BudgetingSystem.Shared.Infrastructure.Repositories;

public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly List<T> _entities = new();

    public Task<T?> GetByIdAsync(Guid id)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(entity);
    }

    public Task<IEnumerable<T>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<T>>(_entities.ToList());
    }

    public Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        _entities.Add(entity);
        return Task.FromResult(entity);
    }

    public Task<T> UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        var existingEntity = _entities.FirstOrDefault(e => e.Id == entity.Id);
        if (existingEntity != null)
        {
            var index = _entities.IndexOf(existingEntity);
            _entities[index] = entity;
        }
        return Task.FromResult(entity);
    }

    public Task DeleteAsync(Guid id)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        if (entity != null)
        {
            _entities.Remove(entity);
        }
        return Task.CompletedTask;
    }
}