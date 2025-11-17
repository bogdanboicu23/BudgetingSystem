using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.DomainEvents;

public record BudgetExceededEvent : IDomainEvent
{
    public DateTime OccurredOn { get; init; }
    public BudgetId BudgetId { get; init; }
    public Money SpentAmount { get; init; }
    public Money BudgetLimit { get; init; }
    public Money ExceedAmount { get; init; }

    public BudgetExceededEvent(BudgetId budgetId, Money spentAmount, Money budgetLimit)
    {
        OccurredOn = DateTime.UtcNow;
        BudgetId = budgetId;
        SpentAmount = spentAmount;
        BudgetLimit = budgetLimit;
        ExceedAmount = Money.Subtract(spentAmount, budgetLimit);
    }
}