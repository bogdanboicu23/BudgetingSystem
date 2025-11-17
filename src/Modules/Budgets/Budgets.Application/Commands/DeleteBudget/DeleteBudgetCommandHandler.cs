using MediatR;
using Budgets.Domain.Repositories;

namespace Budgets.Application.Commands.DeleteBudget;

public class DeleteBudgetCommandHandler : IRequestHandler<DeleteBudgetCommand, Unit>
{
    private readonly IBudgetRepository _budgetRepository;

    public DeleteBudgetCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Unit> Handle(DeleteBudgetCommand request, CancellationToken cancellationToken)
    {
        var budgetExists = await _budgetRepository.ExistsAsync(request.Id, cancellationToken);

        if (!budgetExists)
        {
            throw new ArgumentException($"Budget with ID {request.Id} not found");
        }

        await _budgetRepository.DeleteAsync(request.Id, cancellationToken);

        return Unit.Value;
    }
}