using MediatR;

namespace BudgetService.Domain.DomainEvents;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}