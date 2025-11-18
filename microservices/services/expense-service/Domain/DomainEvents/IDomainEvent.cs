namespace ExpenseService.Domain.DomainEvents;

public interface IDomainEvent
{
    Guid Id { get; }
    DateTime OccurredAt { get; }
}