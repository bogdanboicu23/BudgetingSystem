using MediatR;
using BudgetService.Application.DTOs;

namespace BudgetService.Application.Commands;

public record CreateBudgetCommand : IRequest<BudgetDto>
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string PeriodType { get; init; } = string.Empty;
    public List<CreateBudgetCategoryDto> Categories { get; init; } = new();
}

public record CreateBudgetCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public decimal AllocatedAmount { get; init; }
}