using MediatR;
using Budgets.Domain.Entities;
using Budgets.Domain.Repositories;

namespace Budgets.Application.Queries.GetBudgetById;

public class GetBudgetByIdQueryHandler : IRequestHandler<GetBudgetByIdQuery, Budget?>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetByIdQueryHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Budget?> Handle(GetBudgetByIdQuery request, CancellationToken cancellationToken)
    {
        return await _budgetRepository.GetByIdAsync(request.Id, cancellationToken);
    }
}