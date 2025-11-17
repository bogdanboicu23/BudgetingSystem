using Budgets.Domain.Facades;
using Budgets.Domain.Decorators;
using Budgets.Application.Queries.GetBudgetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetAnalyticsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly BudgetAnalyticsFacade _analyticsFacade;

    public BudgetAnalyticsController(IMediator mediator, BudgetAnalyticsFacade analyticsFacade)
    {
        _mediator = mediator;
        _analyticsFacade = analyticsFacade;
    }

    /// <summary>
    /// Generate comprehensive budget summary report using Facade pattern
    /// </summary>
    [HttpPost("summary/{budgetId}")]
    public async Task<ActionResult> GenerateBudgetSummary(Guid budgetId, [FromBody] BudgetEnhancementOptionsDto options)
    {
        var budgetQuery = new GetBudgetByIdQuery(budgetId);
        var budget = await _mediator.Send(budgetQuery);

        if (budget == null)
            return NotFound($"Budget with ID {budgetId} not found");

        var enhancementOptions = new BudgetEnhancementOptions
        {
            EnablePlannedTracking = options.EnablePlannedTracking,
            PlannedAdjustment = options.PlannedAdjustment,
            PlannedNote = options.PlannedNote,
            EnableSpentTracking = options.EnableSpentTracking,
            IncludeProjectedSpending = options.IncludeProjectedSpending,
            EnableRemainingAnalysis = options.EnableRemainingAnalysis,
            ReserveAmount = options.ReserveAmount,
            EnableSmartAlerts = options.EnableSmartAlerts
        };

        var enhancedBudget = _analyticsFacade.CreateEnhancedBudget(budget, enhancementOptions);
        var summaryReport = _analyticsFacade.GenerateSummaryReport(enhancedBudget);

        return Ok(summaryReport);
    }

    /// <summary>
    /// Get budget alerts using Facade pattern
    /// </summary>
    [HttpGet("alerts/{budgetId}")]
    public async Task<ActionResult> GetBudgetAlerts(Guid budgetId)
    {
        var budgetQuery = new GetBudgetByIdQuery(budgetId);
        var budget = await _mediator.Send(budgetQuery);

        if (budget == null)
            return NotFound($"Budget with ID {budgetId} not found");

        var enhancementOptions = new BudgetEnhancementOptions
        {
            EnablePlannedTracking = true,
            EnableSpentTracking = true,
            EnableRemainingAnalysis = true,
            EnableSmartAlerts = true
        };

        var enhancedBudget = _analyticsFacade.CreateEnhancedBudget(budget, enhancementOptions);
        var alerts = _analyticsFacade.GenerateAlerts(enhancedBudget);

        return Ok(alerts);
    }

    /// <summary>
    /// Calculate budget performance metrics using Facade pattern
    /// </summary>
    [HttpGet("performance/{budgetId}")]
    public async Task<ActionResult> GetPerformanceMetrics(Guid budgetId)
    {
        var budgetQuery = new GetBudgetByIdQuery(budgetId);
        var budget = await _mediator.Send(budgetQuery);

        if (budget == null)
            return NotFound($"Budget with ID {budgetId} not found");

        var enhancementOptions = new BudgetEnhancementOptions
        {
            EnablePlannedTracking = true,
            EnableSpentTracking = true,
            EnableRemainingAnalysis = true,
            IncludeProjectedSpending = true
        };

        var enhancedBudget = _analyticsFacade.CreateEnhancedBudget(budget, enhancementOptions);
        var performanceMetrics = _analyticsFacade.CalculatePerformanceMetrics(enhancedBudget);

        return Ok(performanceMetrics);
    }

    /// <summary>
    /// Get enhanced budget display information
    /// </summary>
    [HttpPost("enhance/{budgetId}")]
    public async Task<ActionResult> GetEnhancedBudgetInfo(Guid budgetId, [FromBody] BudgetEnhancementOptionsDto options)
    {
        var budgetQuery = new GetBudgetByIdQuery(budgetId);
        var budget = await _mediator.Send(budgetQuery);

        if (budget == null)
            return NotFound($"Budget with ID {budgetId} not found");

        var enhancementOptions = new BudgetEnhancementOptions
        {
            EnablePlannedTracking = options.EnablePlannedTracking,
            PlannedAdjustment = options.PlannedAdjustment,
            PlannedNote = options.PlannedNote,
            EnableSpentTracking = options.EnableSpentTracking,
            IncludeProjectedSpending = options.IncludeProjectedSpending,
            EnableRemainingAnalysis = options.EnableRemainingAnalysis,
            ReserveAmount = options.ReserveAmount,
            EnableSmartAlerts = options.EnableSmartAlerts
        };

        var enhancedBudget = _analyticsFacade.CreateEnhancedBudget(budget, enhancementOptions);

        return Ok(new
        {
            Id = enhancedBudget.Id,
            Name = enhancedBudget.Name,
            Category = enhancedBudget.Category,
            TotalAmount = enhancedBudget.GetTotalAmount(),
            PlannedAmount = enhancedBudget.GetPlannedAmount(),
            SpentAmount = enhancedBudget.GetSpentAmount(),
            RemainingAmount = enhancedBudget.GetRemainingAmount(),
            Status = enhancedBudget.GetStatus().ToString(),
            DisplayInfo = enhancedBudget.GetDisplayInfo(),
            PeriodStart = enhancedBudget.PeriodStart,
            PeriodEnd = enhancedBudget.PeriodEnd
        });
    }
}

// DTOs
public class BudgetEnhancementOptionsDto
{
    public bool EnablePlannedTracking { get; set; } = true;
    public decimal PlannedAdjustment { get; set; } = 0;
    public string PlannedNote { get; set; } = "";
    public bool EnableSpentTracking { get; set; } = true;
    public bool IncludeProjectedSpending { get; set; } = false;
    public bool EnableRemainingAnalysis { get; set; } = true;
    public decimal ReserveAmount { get; set; } = 0;
    public bool EnableSmartAlerts { get; set; } = true;
}