namespace Budgets.Domain.Strategies;

public class FixedBudgetStrategy : IBudgetCalculationStrategy
{
    public decimal CalculateAvailableAmount(decimal totalBudget, decimal spentAmount, DateTime periodStart, DateTime periodEnd)
    {
        return Math.Max(0, totalBudget - spentAmount);
    }

    public string GetStrategyName() => "Fixed Budget";
}