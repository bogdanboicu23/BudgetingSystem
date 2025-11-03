using MediatR;
using Budgets.Domain.Entities;

namespace Budgets.Application.Queries.GetBudgetById;

public class GetBudgetByIdQuery : IRequest<Budget?>
{
    public Guid Id { get; set; }

    public GetBudgetByIdQuery(Guid id)
    {
        Id = id;
    }
}