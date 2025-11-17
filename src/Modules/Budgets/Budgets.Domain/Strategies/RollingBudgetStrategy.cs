namespace Budgets.Domain.Strategies;

public class RollingBudgetStrategy : IBudgetCalculationStrategy
{
    public decimal CalculateAvailableAmount(decimal totalBudget, decimal spentAmount, DateTime periodStart, DateTime periodEnd)
    {
        var totalDays = (periodEnd - periodStart).Days;
        var remainingDays = (periodEnd - DateTime.Now).Days;

        if (remainingDays <= 0) return 0;

        var dailyBudget = totalBudget / totalDays;
        var availableForRemainingDays = dailyBudget * remainingDays;

        return Math.Max(0, availableForRemainingDays);
    }

    public string GetStrategyName() => "Rolling Budget";
}