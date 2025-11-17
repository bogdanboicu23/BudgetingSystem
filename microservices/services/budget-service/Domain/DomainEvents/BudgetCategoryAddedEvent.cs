using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.DomainEvents;

public record BudgetCategoryAddedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public BudgetId BudgetId { get; init; }
    public string CategoryName { get; init; }
    public Money AllocatedAmount { get; init; }

    public BudgetCategoryAddedEvent(BudgetId budgetId, string categoryName, Money allocatedAmount)
    {
        OccurredOn = DateTime.UtcNow;
        BudgetId = budgetId;
        CategoryName = categoryName;
        AllocatedAmount = allocatedAmount;
    }
}