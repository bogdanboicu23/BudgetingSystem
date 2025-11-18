using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Domain.DomainEvents;

public class ExpenseAssignedToBudgetEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ExpenseId ExpenseId { get; }
    public BudgetId BudgetId { get; }
    public Money Amount { get; }

    public ExpenseAssignedToBudgetEvent(ExpenseId expenseId, BudgetId budgetId, Money amount)
    {
        ExpenseId = expenseId;
        BudgetId = budgetId;
        Amount = amount;
    }
}