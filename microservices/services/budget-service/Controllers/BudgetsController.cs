using Microsoft.AspNetCore.Mvc;

namespace BudgetService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetsController : ControllerBase
{
    [HttpGet]
    public IActionResult GetBudgets()
    {
        return Ok(new { message = "Budget service is running", service = "budget-service" });
    }

    [HttpGet("{id}")]
    public IActionResult GetBudget(Guid id)
    {
        return Ok(new { id, name = "Sample Budget", amount = 1000 });
    }

    [HttpPost]
    public IActionResult CreateBudget([FromBody] object budget)
    {
        return CreatedAtAction(nameof(GetBudget), new { id = Guid.NewGuid() }, budget);
    }
}