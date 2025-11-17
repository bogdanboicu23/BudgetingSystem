using Budgets.Domain.Entities;

namespace Budgets.Application.Reports;

public class DetailedReportGenerator : AbstractBudgetReportGenerator
{
    protected override async Task<List<Budget>> CollectBudgetData(Guid userId, DateTime startDate, DateTime endDate)
    {
        await Task.Delay(100); // Simulate more complex query for detailed data

        return new List<Budget>
        {
            new Budget(Guid.NewGuid(), "Groceries", startDate, endDate),
            new Budget(Guid.NewGuid(), "Entertainment", startDate, endDate),
            new Budget(Guid.NewGuid(), "Transportation", startDate, endDate),
            new Budget(Guid.NewGuid(), "Utilities", startDate, endDate),
            new Budget(Guid.NewGuid(), "Healthcare", startDate, endDate)
        };
    }

    protected override async Task<BudgetReportData> ProcessData(List<Budget> budgets)
    {
        await Task.Delay(80);

        var data = new BudgetReportData();

        foreach (var budget in budgets)
        {
            var budgetAmount = 1000m;
            var spentAmount = Random.Shared.Next(100, 1200);

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
        await Task.Delay(40);

        var report = "DETAILED BUDGET ANALYSIS\n";
        report += $"Total Budgeted: ${data.TotalBudgeted:F2}\n";
        report += $"Total Spent: ${data.TotalSpent:F2}\n";
        report += $"Remaining: ${data.Remaining:F2}\n\n";

        report += "DETAILED BREAKDOWN:\n";
        foreach (var budget in data.Budgets)
        {
            var spent = data.CategoryBreakdown[budget.Name];
            var budgetAmount = 1000m; // Mock budget amount
            var percentage = (spent / budgetAmount) * 100;
            var status = percentage > 100 ? "OVER BUDGET" : percentage > 80 ? "WARNING" : "ON TRACK";

            report += $"\n{budget.Name}:\n";
            report += $"  Budgeted: ${budgetAmount:F2}\n";
            report += $"  Spent: ${spent:F2}\n";
            report += $"  Remaining: ${(budgetAmount - spent):F2}\n";
            report += $"  Usage: {percentage:F1}% [{status}]\n";
            report += $"  Period: {budget.PeriodStart:yyyy-MM-dd} to {budget.PeriodEnd:yyyy-MM-dd}\n";
        }

        return report;
    }
}