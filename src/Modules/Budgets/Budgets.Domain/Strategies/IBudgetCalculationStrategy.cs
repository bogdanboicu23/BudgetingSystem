namespace Budgets.Domain.Strategies;

public interface IBudgetCalculationStrategy
{
    decimal CalculateAvailableAmount(decimal totalBudget, decimal spentAmount, DateTime periodStart, DateTime periodEnd);
    string GetStrategyName();
}