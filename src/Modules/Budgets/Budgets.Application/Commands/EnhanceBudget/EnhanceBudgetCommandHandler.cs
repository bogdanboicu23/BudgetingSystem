using Budgets.Domain.Decorators;
using Budgets.Domain.Facades;
using Budgets.Domain.Repositories;
using MediatR;

namespace Budgets.Application.Commands.EnhanceBudget;

public class EnhanceBudgetCommandHandler : IRequestHandler<EnhanceBudgetCommand, EnhanceBudgetResponse>
{
    private readonly IBudgetRepository _budgetRepository;
    private readonly BudgetAnalyticsFacade _analyticsFacade;

    public EnhanceBudgetCommandHandler(IBudgetRepository budgetRepository)
    {
        _budgetRepository = budgetRepository;
        _analyticsFacade = new BudgetAnalyticsFacade();
    }

    public async Task<EnhanceBudgetResponse> Handle(EnhanceBudgetCommand request, CancellationToken cancellationToken)
    {
        // Get the budget
        var budget = await _budgetRepository.GetByIdAsync(request.BudgetId);
        if (budget == null)
        {
            throw new ArgumentException($"Budget with ID {request.BudgetId} not found.");
        }

        // Create enhancement options
        var options = new BudgetEnhancementOptions
        {
            EnablePlannedTracking = request.EnablePlannedTracking,
            PlannedAdjustment = request.PlannedAdjustment,
            PlannedNote = request.PlannedNote,
            EnableSpentTracking = request.EnableSpentTracking,
            IncludeProjectedSpending = request.IncludeProjectedSpending,
            EnableRemainingAnalysis = request.EnableRemainingAnalysis,
            ReserveAmount = request.ReserveAmount,
            EnableSmartAlerts = request.EnableSmartAlerts
        };

        // Create enhanced budget using Facade and Decorator patterns
        var enhancedBudget = _analyticsFacade.CreateEnhancedBudget(budget, options);

        // Generate alerts
        var alerts = _analyticsFacade.GenerateAlerts(enhancedBudget);

        // Track applied enhancements
        var appliedEnhancements = new List<string>();
        if (options.EnablePlannedTracking) appliedEnhancements.Add("Planned Tracking");
        if (options.EnableSpentTracking) appliedEnhancements.Add("Spent Tracking");
        if (options.EnableRemainingAnalysis) appliedEnhancements.Add("Remaining Analysis");

        return new EnhanceBudgetResponse
        {
            BudgetId = enhancedBudget.Id,
            BudgetName = enhancedBudget.Name,
            Category = enhancedBudget.Category,
            TotalAmount = enhancedBudget.GetTotalAmount(),
            PlannedAmount = enhancedBudget.GetPlannedAmount(),
            SpentAmount = enhancedBudget.GetSpentAmount(),
            RemainingAmount = enhancedBudget.GetRemainingAmount(),
            Status = enhancedBudget.GetStatus(),
            DisplayInfo = enhancedBudget.GetDisplayInfo(),
            AppliedEnhancements = appliedEnhancements,
            Alerts = alerts.Select(a => new BudgetAlertDto
            {
                Type = a.Type.ToString(),
                Message = a.Message,
                CreatedAt = a.CreatedAt
            }).ToList()
        };
    }
}