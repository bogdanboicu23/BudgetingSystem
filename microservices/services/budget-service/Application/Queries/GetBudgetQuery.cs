using MediatR;
using BudgetService.Application.DTOs;

namespace BudgetService.Application.Queries;

public record GetBudgetQuery : IRequest<BudgetDto?>
{
    public Guid BudgetId { get; init; }
}

public record GetUserBudgetsQuery : IRequest<IEnumerable<BudgetDto>>
{
    public Guid UserId { get; init; }
    public bool OnlyActive { get; init; } = false;
}

public record GetActiveBudgetQuery : IRequest<BudgetDto?>
{
    public Guid UserId { get; init; }
    public DateTime Date { get; init; } = default;

    public GetActiveBudgetQuery()
    {
        Date = DateTime.UtcNow;
    }
}