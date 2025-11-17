using MediatR;
using BudgetService.Application.Queries;
using BudgetService.Application.DTOs;
using BudgetService.Domain.Repositories;
using BudgetService.Domain.ValueObjects;

namespace BudgetService.Application.Handlers;

public class GetBudgetQueryHandler : IRequestHandler<GetBudgetQuery, BudgetDto?>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetBudgetQueryHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<BudgetDto?> Handle(GetBudgetQuery request, CancellationToken cancellationToken)
    {
        var budget = await _budgetRepository.GetByIdAsync(BudgetId.From(request.BudgetId));

        if (budget == null)
            return null;

        return MapToDto(budget);
    }

    private static BudgetDto MapToDto(Domain.Entities.Budget budget)
    {
        return new BudgetDto
        {
            Id = budget.Id.Value,
            UserId = budget.UserId.Value,
            Name = budget.Name,
            TotalAmount = budget.TotalAmount.Amount,
            SpentAmount = budget.SpentAmount.Amount,
            Currency = budget.TotalAmount.Currency,
            Status = budget.Status.ToString(),
            StartDate = budget.Period.StartDate,
            EndDate = budget.Period.EndDate,
            PeriodType = budget.Period.PeriodType.ToString(),
            CreatedAt = budget.CreatedAt,
            LastModifiedAt = budget.LastModifiedAt,
            Categories = budget.Categories.Select(c => new BudgetCategoryDto
            {
                Name = c.Name,
                AllocatedAmount = c.AllocatedAmount.Amount,
                SpentAmount = c.SpentAmount.Amount,
                Currency = c.AllocatedAmount.Currency
            }).ToList()
        };
    }
}

public class GetUserBudgetsQueryHandler : IRequestHandler<GetUserBudgetsQuery, IEnumerable<BudgetDto>>
{
    private readonly IBudgetRepository _budgetRepository;

    public GetUserBudgetsQueryHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<IEnumerable<BudgetDto>> Handle(GetUserBudgetsQuery request, CancellationToken cancellationToken)
    {
        var budgets = request.OnlyActive
            ? await _budgetRepository.GetActiveBudgetsByUserIdAsync(UserId.From(request.UserId))
            : await _budgetRepository.GetByUserIdAsync(UserId.From(request.UserId));

        return budgets.Select(MapToDto);
    }

    private static BudgetDto MapToDto(Domain.Entities.Budget budget)
    {
        return new BudgetDto
        {
            Id = budget.Id.Value,
            UserId = budget.UserId.Value,
            Name = budget.Name,
            TotalAmount = budget.TotalAmount.Amount,
            SpentAmount = budget.SpentAmount.Amount,
            Currency = budget.TotalAmount.Currency,
            Status = budget.Status.ToString(),
            StartDate = budget.Period.StartDate,
            EndDate = budget.Period.EndDate,
            PeriodType = budget.Period.PeriodType.ToString(),
            CreatedAt = budget.CreatedAt,
            LastModifiedAt = budget.LastModifiedAt,
            Categories = budget.Categories.Select(c => new BudgetCategoryDto
            {
                Name = c.Name,
                AllocatedAmount = c.AllocatedAmount.Amount,
                SpentAmount = c.SpentAmount.Amount,
                Currency = c.AllocatedAmount.Currency
            }).ToList()
        };
    }
}