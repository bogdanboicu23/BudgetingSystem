namespace ReportingService.Application.DTOs;

public class SpendingReportDto
{
    public Guid UserId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalSpent { get; set; }
    public List<CategorySpendingDto> CategoryBreakdown { get; set; } = new();
    public List<MonthlySpendingDto> MonthlyBreakdown { get; set; } = new();
}

public class CategorySpendingDto
{
    public string Category { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Count { get; set; }
}

public class MonthlySpendingDto
{
    public string Month { get; set; } = string.Empty;
    public decimal Amount { get; set; }
}

public class BudgetUsageReportDto
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public decimal BudgetLimit { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public double UsagePercentage { get; set; }
}