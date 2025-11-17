using MediatR;
using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;

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

        await _budgetRepository.AddAsync(budget, cancellationToken);

        return budget.Id;
    }
}

