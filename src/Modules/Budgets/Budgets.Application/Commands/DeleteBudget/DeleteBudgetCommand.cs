using MediatR;

namespace Budgets.Application.Commands.DeleteBudget;

public class DeleteBudgetCommand : IRequest<Unit>
{
    public Guid Id { get; set; }

    public DeleteBudgetCommand(Guid id)
    {
        Id = id;
    }
}