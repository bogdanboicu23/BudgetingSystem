using Microsoft.AspNetCore.Mvc;
using MediatR;
using BudgetService.Application.Commands;
using BudgetService.Application.Queries;
using BudgetService.Application.DTOs;

namespace BudgetService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<BudgetsController> _logger;

    public BudgetsController(IMediator mediator, ILogger<BudgetsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BudgetDto>> GetBudget(Guid id)
    {
        try
        {
            var budget = await _mediator.Send(new GetBudgetQuery { BudgetId = id });

            if (budget == null)
                return NotFound($"Budget with ID {id} not found");

            return Ok(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budget {BudgetId}", id);
            return StatusCode(500, "An error occurred while retrieving the budget");
        }
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<BudgetDto>>> GetUserBudgets(Guid userId, [FromQuery] bool onlyActive = false)
    {
        try
        {
            var budgets = await _mediator.Send(new GetUserBudgetsQuery { UserId = userId, OnlyActive = onlyActive });
            return Ok(budgets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting budgets for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the budgets");
        }
    }

    [HttpGet("user/{userId}/active")]
    public async Task<ActionResult<BudgetDto>> GetActiveBudget(Guid userId, [FromQuery] DateTime? date = null)
    {
        try
        {
            var query = new GetActiveBudgetQuery { UserId = userId };
            if (date.HasValue)
            {
                query = query with { Date = date.Value };
            }

            var budget = await _mediator.Send(query);

            if (budget == null)
                return NotFound($"No active budget found for user {userId}");

            return Ok(budget);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active budget for user {UserId}", userId);
            return StatusCode(500, "An error occurred while retrieving the active budget");
        }
    }

    [HttpPost]
    public async Task<ActionResult<BudgetDto>> CreateBudget([FromBody] CreateBudgetCommand command)
    {
        try
        {
            var budget = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetBudget), new { id = budget.Id }, budget);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument for creating budget");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for creating budget");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating budget");
            return StatusCode(500, "An error occurred while creating the budget");
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<BudgetDto>> UpdateBudget(Guid id, [FromBody] UpdateBudgetCommand command)
    {
        try
        {
            if (id != command.BudgetId)
                return BadRequest("Budget ID mismatch");

            var budget = await _mediator.Send(command);
            return Ok(budget);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument for updating budget {BudgetId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for updating budget {BudgetId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating budget {BudgetId}", id);
            return StatusCode(500, "An error occurred while updating the budget");
        }
    }

    [HttpPost("{id}/expenses")]
    public async Task<ActionResult> RecordExpense(Guid id, [FromBody] RecordExpenseCommand command)
    {
        try
        {
            if (id != command.BudgetId)
                return BadRequest("Budget ID mismatch");

            await _mediator.Send(command);
            return Ok();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument for recording expense in budget {BudgetId}", id);
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation for recording expense in budget {BudgetId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording expense in budget {BudgetId}", id);
            return StatusCode(500, "An error occurred while recording the expense");
        }
    }

    [HttpGet]
    public async Task<ActionResult> Health()
    {
        return Ok(new { service = "budget-service", status = "healthy", timestamp = DateTime.UtcNow });
    }
}