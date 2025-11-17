using Budgets.Domain.Decorators;

namespace Budgets.Domain.Entities;

public class BudgetPerformanceSummary
{
    public double TimeProgress { get; set; }
    public double SpendingProgress { get; set; }
    public double IncomeProgress { get; set; }
    public decimal RemainingBudget { get; set; }
    public decimal IncomeVariance { get; set; }
    public decimal ExpenseVariance { get; set; }
    public BudgetStatus Status { get; set; }

    public bool IsOnTrack => Math.Abs(SpendingProgress - TimeProgress) <= 0.1; // Within 10%
    public bool IsOverspending => SpendingProgress > TimeProgress + 0.1;
    public bool IsUnderspending => SpendingProgress < TimeProgress - 0.1;

    public string GetRecommendation()
    {
        if (IsOverspending)
        {
            return "Consider reducing expenses to stay within budget";
        }
        else if (IsUnderspending && TimeProgress > 0.8)
        {
            return "You have room to increase spending or save more";
        }
        else if (IncomeProgress < 0.5 && TimeProgress > 0.5)
        {
            return "Income is behind schedule - review income sources";
        }
        else if (IsOnTrack)
        {
            return "Budget is on track - great job!";
        }
        else
        {
            return "Continue monitoring your budget progress";
        }
    }
}