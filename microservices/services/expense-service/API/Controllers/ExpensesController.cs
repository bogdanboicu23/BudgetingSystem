using ExpenseService.Application.Commands;
using ExpenseService.Application.DTOs;
using ExpenseService.Application.Handlers;

namespace ExpenseService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExpensesController : ControllerBase
{
    private readonly CreateExpenseCommandHandler _createExpenseHandler;

    public ExpensesController(CreateExpenseCommandHandler createExpenseHandler)
    {
        _createExpenseHandler = createExpenseHandler;
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseDto>> CreateExpense(CreateExpenseDto createExpenseDto)
    {
        var command = new CreateExpenseCommand
        {
            UserId = createExpenseDto.UserId,
            BudgetId = createExpenseDto.BudgetId,
            Description = createExpenseDto.Description,
            Amount = createExpenseDto.Amount,
            Currency = createExpenseDto.Currency,
            Category = createExpenseDto.Category,
            Date = createExpenseDto.Date,
            Merchant = createExpenseDto.Merchant,
            Notes = createExpenseDto.Notes
        };

        var expense = await _createExpenseHandler.Handle(command);
        return CreatedAtAction(nameof(GetExpense), new { id = expense.Id }, expense);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ExpenseDto>> GetExpense(Guid id)
    {
        // Placeholder - would implement GetExpenseQueryHandler
        return Ok();
    }
}