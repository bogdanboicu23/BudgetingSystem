using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.DomainEvents;

public record BudgetAmountUpdatedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public BudgetId BudgetId { get; init; }
    public Money NewAmount { get; init; }

    public BudgetAmountUpdatedEvent(BudgetId budgetId, Money newAmount)
    {
        OccurredOn = DateTime.UtcNow;
        BudgetId = budgetId;
        NewAmount = newAmount;
    }
}