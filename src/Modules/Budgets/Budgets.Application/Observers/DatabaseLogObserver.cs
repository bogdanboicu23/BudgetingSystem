using Budgets.Domain.Events;

namespace Budgets.Application.Observers;

public class DatabaseLogObserver : IBudgetObserver
{
    public async Task OnBudgetLimitExceeded(Guid budgetId, decimal currentAmount, decimal limitAmount)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            EventType = "BUDGET_EXCEEDED",
            BudgetId = budgetId,
            CurrentAmount = currentAmount,
            LimitAmount = limitAmount
        };

        Console.WriteLine($"DB LOG: {logEntry.EventType} for budget {budgetId}");
        await Task.Delay(50);
    }

    public async Task OnBudgetWarningThreshold(Guid budgetId, decimal currentAmount, decimal warningThreshold)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            EventType = "BUDGET_WARNING",
            BudgetId = budgetId,
            CurrentAmount = currentAmount,
            WarningThreshold = warningThreshold
        };

        Console.WriteLine($"DB LOG: {logEntry.EventType} for budget {budgetId}");
        await Task.Delay(50);
    }

    public async Task OnBudgetGoalReached(Guid budgetId, decimal targetAmount)
    {
        var logEntry = new
        {
            Timestamp = DateTime.UtcNow,
            EventType = "BUDGET_GOAL_REACHED",
            BudgetId = budgetId,
            TargetAmount = targetAmount
        };

        Console.WriteLine($"DB LOG: {logEntry.EventType} for budget {budgetId}");
        await Task.Delay(50);
    }
}