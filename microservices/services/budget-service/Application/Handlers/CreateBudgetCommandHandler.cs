using MediatR;
using BudgetService.Application.Commands;
using BudgetService.Application.DTOs;
using BudgetService.Domain.Entities;
using BudgetService.Domain.Repositories;
using BudgetService.Domain.ValueObjects;

namespace BudgetService.Application.Handlers;

public class CreateBudgetCommandHandler : IRequestHandler<CreateBudgetCommand, BudgetDto>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly IMediator _mediator;

    public CreateBudgetCommandHandler(IBudgetRepository budgetRepository, IMediator mediator)
    {
        _budgetRepository = budgetRepository;
        _mediator = mediator;
    }

    public async Task<BudgetDto> Handle(CreateBudgetCommand request, CancellationToken cancellationToken)
    {
        var userId = UserId.From(request.UserId);
        var money = Money.Create(request.Amount, request.Currency);
        var period = CreateBudgetPeriod(request);

        var budget = new Budget(userId, request.Name, money, period);

        foreach (var categoryRequest in request.Categories)
        {
            var categoryMoney = Money.Create(categoryRequest.AllocatedAmount, request.Currency);
            budget.AddCategory(categoryRequest.Name, categoryMoney);
        }

        await _budgetRepository.AddAsync(budget);

        foreach (var domainEvent in budget.DomainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        budget.ClearDomainEvents();

        return MapToDto(budget);
    }

    private static BudgetPeriod CreateBudgetPeriod(CreateBudgetCommand request)
    {
        return request.PeriodType.ToLower() switch
        {
            "monthly" => BudgetPeriod.Monthly(request.StartDate),
            "yearly" => BudgetPeriod.Yearly(request.StartDate),
            "weekly" => BudgetPeriod.Weekly(request.StartDate),
            _ => BudgetPeriod.Create(request.StartDate, request.EndDate, BudgetPeriodType.Custom)
        };
    }

    private static BudgetDto MapToDto(Budget budget)
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