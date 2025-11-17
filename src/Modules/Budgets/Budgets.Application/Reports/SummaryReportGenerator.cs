using Budgets.Domain.Entities;

namespace Budgets.Application.Reports;

public class SummaryReportGenerator : AbstractBudgetReportGenerator
{
    protected override async Task<List<Budget>> CollectBudgetData(Guid userId, DateTime startDate, DateTime endDate)
    {
        await Task.Delay(50); // Simulate database query

        // Mock data - in real implementation, query from repository
        return new List<Budget>
        {
            new Budget(Guid.NewGuid(), "Groceries", startDate, endDate),
            new Budget(Guid.NewGuid(), "Entertainment", startDate, endDate),
            new Budget(Guid.NewGuid(), "Transportation", startDate, endDate)
        };
    }

    protected override async Task<BudgetReportData> ProcessData(List<Budget> budgets)
    {
        await Task.Delay(30);

        var data = new BudgetReportData();

        foreach (var budget in budgets)
        {
            // Mock calculations - in real implementation, calculate from expenses
            var budgetAmount = 1000m; // Would get from budget entity
            var spentAmount = Random.Shared.Next(200, 800);

            data.TotalBudgeted += budgetAmount;
            data.TotalSpent += spentAmount;
            data.CategoryBreakdown[budget.Name] = spentAmount;
        }

        data.Remaining = data.TotalBudgeted - data.TotalSpent;
        data.Budgets = budgets;

        return data;
    }

    protected override async Task<string> FormatReport(BudgetReportData data)
    {
        await Task.Delay(20);

        var report = "BUDGET SUMMARY\n";
        report += $"Total Budgeted: ${data.TotalBudgeted:F2}\n";
        report += $"Total Spent: ${data.TotalSpent:F2}\n";
        report += $"Remaining: ${data.Remaining:F2}\n";
        report += $"Savings Rate: {((data.Remaining / data.TotalBudgeted) * 100):F1}%\n\n";

        report += "CATEGORY BREAKDOWN:\n";
        foreach (var category in data.CategoryBreakdown)
        {
            report += $"  {category.Key}: ${category.Value:F2}\n";
        }

        return report;
    }
}