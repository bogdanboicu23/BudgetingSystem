using MediatR;
using Budgets.Application.Commands.CreateBudget;

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
        var budget = await _budgetRepository.GetByIdAsync(request.Id);

        if (budget == null)
        {
            throw new ArgumentException($"Budget with ID {request.Id} not found");
        }

        await _budgetRepository.DeleteAsync(budget);
        await _budgetRepository.SaveChangesAsync();

        return Unit.Value;
    }
}