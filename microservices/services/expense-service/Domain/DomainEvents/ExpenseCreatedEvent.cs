using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Domain.DomainEvents;

public class ExpenseCreatedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ExpenseId ExpenseId { get; }
    public UserId UserId { get; }
    public Money Amount { get; }
    public ExpenseCategory Category { get; }
    public DateTime ExpenseDate { get; }
    public BudgetId? BudgetId { get; }

    public ExpenseCreatedEvent(
        ExpenseId expenseId,
        UserId userId,
        Money amount,
        ExpenseCategory category,
        DateTime expenseDate,
        BudgetId? budgetId)
    {
        ExpenseId = expenseId;
        UserId = userId;
        Amount = amount;
        Category = category;
        ExpenseDate = expenseDate;
        BudgetId = budgetId;
    }
}