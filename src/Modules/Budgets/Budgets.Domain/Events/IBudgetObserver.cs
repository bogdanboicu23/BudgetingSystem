namespace Budgets.Domain.Events;

public interface IBudgetObserver
{
    Task OnBudgetLimitExceeded(Guid budgetId, decimal currentAmount, decimal limitAmount);
    Task OnBudgetWarningThreshold(Guid budgetId, decimal currentAmount, decimal warningThreshold);
    Task OnBudgetGoalReached(Guid budgetId, decimal targetAmount);
}