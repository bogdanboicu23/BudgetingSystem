using Microsoft.AspNetCore.Mvc;
using BudgetingSystem.Budgets.Application.Services;

namespace BudgetingSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly BudgetService _budgetService;

    public BudgetsController(BudgetService budgetService)
    {
        _budgetService = budgetService;
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserBudgets(Guid userId)
    {
        var budgets = await _budgetService.GetUserBudgetsAsync(userId);
        return Ok(budgets);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBudget(Guid id)
    {
        var budget = await _budgetService.GetBudgetByIdAsync(id);
        if (budget == null)
            return NotFound();
        return Ok(budget);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetRequest request)
    {
        var budget = await _budgetService.CreateBudgetAsync(
            request.UserId,
            request.Name,
            request.Description,
            request.TotalAmount,
            request.StartDate,
            request.EndDate);

        return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
    }

    [HttpPut("{id}/spent")]
    public async Task<IActionResult> UpdateSpentAmount(Guid id, [FromBody] UpdateSpentAmountRequest request)
    {
        try
        {
            var budget = await _budgetService.UpdateBudgetSpentAmountAsync(id, request.SpentAmount);
            return Ok(budget);
        }
        catch (ArgumentException)
        {
            return NotFound();
        }
    }
}

public record CreateBudgetRequest(Guid UserId, string Name, string Description, decimal TotalAmount, DateTime StartDate, DateTime EndDate);
public record UpdateSpentAmountRequest(decimal SpentAmount);