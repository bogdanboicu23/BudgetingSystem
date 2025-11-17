using MediatR;
using BudgetService.Application.DTOs;

namespace BudgetService.Application.Commands;

public record UpdateBudgetCommand : IRequest<BudgetDto>
{
    public Guid BudgetId { get; init; }
    public string? Name { get; init; }
    public decimal? Amount { get; init; }
    public string? Currency { get; init; }
}