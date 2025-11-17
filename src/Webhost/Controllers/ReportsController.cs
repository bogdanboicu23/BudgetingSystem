using Budgets.Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Webhost.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly SummaryReportGenerator _summaryGenerator;
    private readonly DetailedReportGenerator _detailedGenerator;

    public ReportsController(
        SummaryReportGenerator summaryGenerator,
        DetailedReportGenerator detailedGenerator)
    {
        _summaryGenerator = summaryGenerator;
        _detailedGenerator = detailedGenerator;
    }

    /// <summary>
    /// Generate summary report using Template Method pattern
    /// </summary>
    [HttpGet("summary/{userId}")]
    public async Task<ActionResult> GetSummaryReport(
        Guid userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        startDate ??= DateTime.Now.AddDays(-30);
        endDate ??= DateTime.Now;

        var report = await _summaryGenerator.GenerateReportAsync(userId, startDate.Value, endDate.Value);
        return Ok(new { Report = report, Type = "Summary" });
    }

    /// <summary>
    /// Generate detailed report using Template Method pattern
    /// </summary>
    [HttpGet("detailed/{userId}")]
    public async Task<ActionResult> GetDetailedReport(
        Guid userId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate)
    {
        startDate ??= DateTime.Now.AddDays(-30);
        endDate ??= DateTime.Now;

        var report = await _detailedGenerator.GenerateReportAsync(userId, startDate.Value, endDate.Value);
        return Ok(new { Report = report, Type = "Detailed" });
    }
}