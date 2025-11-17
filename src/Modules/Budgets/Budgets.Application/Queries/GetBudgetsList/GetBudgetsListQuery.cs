using MediatR;
using Budgets.Domain.Entities;

namespace Budgets.Application.Queries.GetBudgetsList;

public class GetBudgetsListQuery : IRequest<IList<Budget>>
{
}