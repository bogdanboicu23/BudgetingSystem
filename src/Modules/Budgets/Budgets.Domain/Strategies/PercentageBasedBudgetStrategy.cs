namespace Budgets.Domain.Strategies;

public class PercentageBasedBudgetStrategy : IBudgetCalculationStrategy
{
    private readonly decimal _warningThreshold;

    public PercentageBasedBudgetStrategy(decimal warningThreshold = 0.8m)
    {
        _warningThreshold = warningThreshold;
    }

    public decimal CalculateAvailableAmount(decimal totalBudget, decimal spentAmount, DateTime periodStart, DateTime periodEnd)
    {
        var remaining = totalBudget - spentAmount;
        var spentPercentage = spentAmount / totalBudget;

        if (spentPercentage >= _warningThreshold)
        {
            var conservativeAmount = remaining * 0.5m;
            return Math.Max(0, conservativeAmount);
        }

        return Math.Max(0, remaining);
    }

    public string GetStrategyName() => "Percentage-Based Budget";
}