using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Domain.DomainEvents;

public class ExpenseAmountUpdatedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ExpenseId ExpenseId { get; }
    public Money OldAmount { get; }
    public Money NewAmount { get; }

    public ExpenseAmountUpdatedEvent(ExpenseId expenseId, Money oldAmount, Money newAmount)
    {
        ExpenseId = expenseId;
        OldAmount = oldAmount;
        NewAmount = newAmount;
    }
}