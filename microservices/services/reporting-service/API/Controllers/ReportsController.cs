using ReportingService.Application.DTOs;

namespace ReportingService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    [HttpGet("spending/{userId}")]
    public async Task<ActionResult<SpendingReportDto>> GetSpendingReport(Guid userId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
    {
        // Placeholder implementation
        var report = new SpendingReportDto
        {
            UserId = userId,
            StartDate = startDate ?? DateTime.UtcNow.AddMonths(-1),
            EndDate = endDate ?? DateTime.UtcNow,
            TotalSpent = 1500.00m,
            CategoryBreakdown = new List<CategorySpendingDto>
            {
                new() { Category = "Food", Amount = 500, Count = 15 },
                new() { Category = "Transportation", Amount = 300, Count = 8 }
            },
            MonthlyBreakdown = new List<MonthlySpendingDto>
            {
                new() { Month = "January", Amount = 1500 }
            }
        };

        return Ok(report);
    }

    [HttpGet("budget-usage/{budgetId}")]
    public async Task<ActionResult<BudgetUsageReportDto>> GetBudgetUsageReport(Guid budgetId)
    {
        // Placeholder implementation
        var report = new BudgetUsageReportDto
        {
            BudgetId = budgetId,
            BudgetName = "Monthly Budget",
            BudgetLimit = 2000.00m,
            SpentAmount = 1500.00m,
            RemainingAmount = 500.00m,
            UsagePercentage = 75.0
        };

        return Ok(report);
    }
}