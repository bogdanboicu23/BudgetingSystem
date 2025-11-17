using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.DomainEvents;

public record BudgetCreatedEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public BudgetId BudgetId { get; init; }
    public UserId UserId { get; init; }
    public string BudgetName { get; init; }
    public Money TotalAmount { get; init; }

    public BudgetCreatedEvent(BudgetId budgetId, UserId userId, string budgetName, Money totalAmount)
    {
        OccurredOn = DateTime.UtcNow;
        BudgetId = budgetId;
        UserId = userId;
        BudgetName = budgetName;
        TotalAmount = totalAmount;
    }
}