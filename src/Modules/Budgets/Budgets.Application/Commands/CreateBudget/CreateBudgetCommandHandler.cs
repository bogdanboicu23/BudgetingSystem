using MediatR;
using Budgets.Domain.Entities;

namespace Budgets.Application.Commands.CreateBudget;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, Guid>
{
    private readonly IBudgetRepository _budgetRepository;

    public CreateBudgetCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var budget = new Budget(
            Guid.NewGuid(),
            request.Name,
            request.PeriodStart,
            request.PeriodEnd
        );

        await _budgetRepository.AddAsync(budget);
        await _budgetRepository.SaveChangesAsync();

        return budget.Id;
    }
}

public interface IBudgetRepository
{
    Task<Budget?> GetByIdAsync(Guid id);
    Task<IList<Budget>> GetAllAsync();
    Task AddAsync(Budget budget);
    Task UpdateAsync(Budget budget);
    Task DeleteAsync(Budget budget);
    Task SaveChangesAsync();
}