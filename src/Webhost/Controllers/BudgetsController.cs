using Budgets.Application.Commands.CreateBudget;
using Budgets.Application.Commands.DeleteBudget;
using Budgets.Application.Commands.EnhanceBudget;
using Budgets.Application.Queries.GetBudgetById;
using Budgets.Application.Queries.GetBudgetsList;
using Budgets.Domain.Strategies;
using Budgets.Domain.Facades;
using Budgets.Domain.Decorators;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BudgetsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Get all budgets
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetBudgets()
    {
        var query = new GetBudgetsListQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Get budget by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetBudget(Guid id)
    {
        var query = new GetBudgetByIdQuery(id);
        var result = await _mediator.Send(query);

        if (result == null)
            return NotFound($"Budget with ID {id} not found");

        return Ok(result);
    }

    /// <summary>
    /// Create a new budget
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetBudget), new { id = result }, result);
    }

    /// <summary>
    /// Delete a budget
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteBudget(Guid id)
    {
        var command = new DeleteBudgetCommand(id);
        await _mediator.Send(command);
        return NoContent();
    }


    /// <summary>
    /// Calculate budget using Strategy pattern
    /// </summary>
    [HttpPost("calculate")]
    public async Task<ActionResult> CalculateBudget(
        [FromBody] BudgetCalculationRequest request,
        [FromServices] FixedBudgetStrategy fixedStrategy,
        [FromServices] RollingBudgetStrategy rollingStrategy,
        [FromServices] PercentageBasedBudgetStrategy percentageStrategy)
    {
        IBudgetCalculationStrategy strategy = request.StrategyType?.ToLower() switch
        {
            "rolling" => rollingStrategy,
            "percentagebased" => percentageStrategy,
            _ => fixedStrategy
        };

        var totalBudget = request.TotalBudget > 0 ? request.TotalBudget : 1000m;
        var available = strategy.CalculateAvailableAmount(
            totalBudget,
            request.CurrentSpending,
            DateTime.Now.AddDays(-15),
            DateTime.Now.AddDays(15));
        var spentPercentage = (request.CurrentSpending / totalBudget) * 100;

        var warnings = new List<string>();
        if (spentPercentage >= 100) warnings.Add("Budget exceeded!");
        else if (spentPercentage >= 80) warnings.Add("Approaching budget limit");

        return Ok(new
        {
            AvailableAmount = available,
            TotalBudget = totalBudget,
            CurrentSpending = request.CurrentSpending,
            StrategyUsed = strategy.GetStrategyName(),
            SpentPercentage = spentPercentage,
            Warnings = warnings
        });
    }

    /// <summary>
    /// Enhance budget using Decorator pattern
    /// </summary>
    [HttpPost("{id}/enhance")]
    public async Task<ActionResult> EnhanceBudget(Guid id, [FromBody] EnhanceBudgetRequest request)
    {
        var command = new EnhanceBudgetCommand
        {
            BudgetId = id,
            EnablePlannedTracking = request.EnablePlannedTracking,
            PlannedAdjustment = request.PlannedAdjustment,
            PlannedNote = request.PlannedNote,
            EnableSpentTracking = request.EnableSpentTracking,
            IncludeProjectedSpending = request.IncludeProjectedSpending,
            EnableRemainingAnalysis = request.EnableRemainingAnalysis,
            ReserveAmount = request.ReserveAmount,
            EnableSmartAlerts = request.EnableSmartAlerts
        };

        var result = await _mediator.Send(command);
        return Ok(result);
    }
}

// DTOs
public class EnhanceBudgetRequest
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

public class BudgetCalculationRequest
{
    public Guid BudgetId { get; set; }
    public string StrategyType { get; set; } = "Fixed";
    public decimal TotalBudget { get; set; }
    public decimal CurrentSpending { get; set; }
}