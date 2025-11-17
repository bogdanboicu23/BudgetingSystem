using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.DomainEvents;

public record BudgetExpenseRecordedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public BudgetId BudgetId { get; init; }
    public Money Amount { get; init; }
    public string? CategoryName { get; init; }

    public BudgetExpenseRecordedEvent(BudgetId budgetId, Money amount, string? categoryName)
    {
        OccurredOn = DateTime.UtcNow;
        BudgetId = budgetId;
        Amount = amount;
        CategoryName = categoryName;
    }
}