using MediatR;
using Budgets.Domain.Entities;
using Budgets.Application.Commands.CreateBudget;

namespace Budgets.Application.Queries.GetBudgetsList;

public class GetBudgetsListQueryHandler : IRequestHandler<GetBudgetsListQuery, IList<Budget>>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetsListQueryHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<IList<Budget>> Handle(GetBudgetsListQuery request, CancellationToken cancellationToken)
    {
        return await _budgetRepository.GetAllAsync();
    }
}