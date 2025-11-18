using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Domain.DomainEvents;

public class ExpenseCategoryUpdatedEvent : IDomainEvent
{
    public Guid Id { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;

    public ExpenseId ExpenseId { get; }
    public ExpenseCategory OldCategory { get; }
    public ExpenseCategory NewCategory { get; }

    public ExpenseCategoryUpdatedEvent(ExpenseId expenseId, ExpenseCategory oldCategory, ExpenseCategory newCategory)
    {
        ExpenseId = expenseId;
        OldCategory = oldCategory;
        NewCategory = newCategory;
    }
}