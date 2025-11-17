using MediatR;

namespace BudgetService.Application.Commands;

public record RecordExpenseCommand : IRequest<Unit>
{
    public Guid BudgetId { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string? CategoryName { get; init; }
    public string Description { get; init; } = string.Empty;
}