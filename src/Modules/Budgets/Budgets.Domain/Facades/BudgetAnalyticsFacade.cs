using Budgets.Domain.Decorators;
using Budgets.Domain.Entities;

namespace Budgets.Domain.Facades;

public class BudgetAnalyticsFacade
{
    public IBudgetComponent CreateEnhancedBudget(Budget budget, BudgetEnhancementOptions options)
    {
        IBudgetComponent component = new BaseBudgetComponent(budget);

        // Apply decorators based on options
        if (options.EnablePlannedTracking)
        {
            component = new PlannedBudgetDecorator(
                component,
                options.PlannedAdjustment,
                options.PlannedNote);
        }

        if (options.EnableSpentTracking)
        {
            component = new SpentTrackingDecorator(
                component,
                options.IncludeProjectedSpending);
        }

        if (options.EnableRemainingAnalysis)
        {
            component = new RemainingBudgetDecorator(
                component,
                options.ReserveAmount,
                options.EnableSmartAlerts);
        }

        return component;
    }

    public BudgetSummaryReport GenerateSummaryReport(IBudgetComponent budget)
    {
        return new BudgetSummaryReport
        {
            BudgetId = budget.Id,
            BudgetName = budget.Name,
            Category = budget.Category,
            TotalAmount = budget.GetTotalAmount(),
            PlannedAmount = budget.GetPlannedAmount(),
            SpentAmount = budget.GetSpentAmount(),
            RemainingAmount = budget.GetRemainingAmount(),
            Status = budget.GetStatus(),
            DisplayInfo = budget.GetDisplayInfo(),
            PeriodStart = budget.PeriodStart,
            PeriodEnd = budget.PeriodEnd,
            GeneratedAt = DateTime.UtcNow
        };
    }

    public List<BudgetAlert> GenerateAlerts(IBudgetComponent budget)
    {
        var alerts = new List<BudgetAlert>();

        // Status-based alerts
        var status = budget.GetStatus();
        switch (status)
        {
            case BudgetStatus.OverBudget:
                alerts.Add(new BudgetAlert
                {
                    Type = AlertType.Critical,
                    Message = $"Budget '{budget.Name}' is over budget by ${budget.GetSpentAmount() - budget.GetTotalAmount():F2}",
                    BudgetId = budget.Id
                });
                break;

            case BudgetStatus.Warning:
                alerts.Add(new BudgetAlert
                {
                    Type = AlertType.Warning,
                    Message = $"Budget '{budget.Name}' is approaching its limit",
                    BudgetId = budget.Id
                });
                break;
        }

        // Smart alerts from decorators
        if (budget is RemainingBudgetDecorator remainingDecorator)
        {
            var smartAlerts = remainingDecorator.GetSmartAlerts();
            alerts.AddRange(smartAlerts.Select(alert => new BudgetAlert
            {
                Type = AlertType.Info,
                Message = $"{budget.Name}: {alert}",
                BudgetId = budget.Id
            }));
        }

        return alerts;
    }

    public BudgetPerformanceMetrics CalculatePerformanceMetrics(IBudgetComponent budget)
    {
        var totalDays = (budget.PeriodEnd - budget.PeriodStart).TotalDays;
        var daysElapsed = (DateTime.Now - budget.PeriodStart).TotalDays;
        var timeProgress = totalDays > 0 ? Math.Min(1, daysElapsed / totalDays) : 0;

        var spendingProgress = budget.GetTotalAmount() > 0 ?
            (double)(budget.GetSpentAmount() / budget.GetTotalAmount()) : 0;

        return new BudgetPerformanceMetrics
        {
            BudgetId = budget.Id,
            TimeProgress = timeProgress,
            SpendingProgress = spendingProgress,
            EfficiencyRatio = timeProgress > 0 ? spendingProgress / timeProgress : 0,
            DailySpendingRate = daysElapsed > 0 ? (double)(budget.GetSpentAmount() / (decimal)daysElapsed) : 0,
            ProjectedEndAmount = CalculateProjectedEndAmount(budget, timeProgress),
            IsOnTrack = Math.Abs(spendingProgress - timeProgress) <= 0.1 // Within 10%
        };
    }

    private decimal CalculateProjectedEndAmount(IBudgetComponent budget, double timeProgress)
    {
        if (timeProgress <= 0) return 0;

        var currentSpendingRate = budget.GetSpentAmount() / (decimal)timeProgress;
        return Math.Min(currentSpendingRate, budget.GetTotalAmount());
    }
}

public class BudgetEnhancementOptions
{
    public bool EnablePlannedTracking { get; set; } = true;
    public decimal PlannedAdjustment { get; set; } = 0;
    public string PlannedNote { get; set; } = "";

    public bool EnableSpentTracking { get; set; } = true;
    public bool IncludeProjectedSpending { get; set; } = false;

    public bool EnableRemainingAnalysis { get; set; } = true;
    public decimal ReserveAmount { get; set; } = 0;
    public bool EnableSmartAlerts { get; set; } = true;
}

public class BudgetSummaryReport
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public BudgetStatus Status { get; set; }
    public string DisplayInfo { get; set; } = string.Empty;
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
    public DateTime GeneratedAt { get; set; }
}

public class BudgetAlert
{
    public AlertType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid BudgetId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public enum AlertType
{
    Info,
    Warning,
    Critical
}

public class BudgetPerformanceMetrics
{
    public Guid BudgetId { get; set; }
    public double TimeProgress { get; set; }
    public double SpendingProgress { get; set; }
    public double EfficiencyRatio { get; set; }
    public double DailySpendingRate { get; set; }
    public decimal ProjectedEndAmount { get; set; }
    public bool IsOnTrack { get; set; }
}