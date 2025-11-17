using Budgets.Domain.Entities;

namespace Budgets.Application.Reports;

public abstract class AbstractBudgetReportGenerator
{
    public async Task<string> GenerateReportAsync(Guid userId, DateTime startDate, DateTime endDate)
    {
        var header = await GenerateHeader(userId, startDate, endDate);
        var budgetData = await CollectBudgetData(userId, startDate, endDate);
        var processedData = await ProcessData(budgetData);
        var formattedReport = await FormatReport(processedData);
        var footer = await GenerateFooter();

        return $"{header}\n{formattedReport}\n{footer}";
    }

    protected virtual async Task<string> GenerateHeader(Guid userId, DateTime startDate, DateTime endDate)
    {
        await Task.Delay(10);
        return $"Budget Report for User: {userId}\nPeriod: {startDate:yyyy-MM-dd} to {endDate:yyyy-MM-dd}\n" +
               new string('=', 50);
    }

    protected virtual async Task<string> GenerateFooter()
    {
        await Task.Delay(10);
        return $"\nReport generated on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
    }

    protected abstract Task<List<Budget>> CollectBudgetData(Guid userId, DateTime startDate, DateTime endDate);
    protected abstract Task<BudgetReportData> ProcessData(List<Budget> budgets);
    protected abstract Task<string> FormatReport(BudgetReportData data);
}

public class BudgetReportData
{
    public decimal TotalBudgeted { get; set; }
    public decimal TotalSpent { get; set; }
    public decimal Remaining { get; set; }
    public List<Budget> Budgets { get; set; } = new();
    public Dictionary<string, decimal> CategoryBreakdown { get; set; } = new();
}