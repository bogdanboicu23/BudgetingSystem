using Budgets.Domain.Events;

namespace Budgets.Application.Observers;

public class EmailNotificationObserver : IBudgetObserver
{
    public async Task OnBudgetLimitExceeded(Guid budgetId, decimal currentAmount, decimal limitAmount)
    {
        Console.WriteLine($"EMAIL ALERT: Budget {budgetId} exceeded! Current: ${currentAmount:F2}, Limit: ${limitAmount:F2}");
        await Task.Delay(100); // Simulate email sending
    }

    public async Task OnBudgetWarningThreshold(Guid budgetId, decimal currentAmount, decimal warningThreshold)
    {
        Console.WriteLine($"EMAIL WARNING: Budget {budgetId} approaching limit. Current: ${currentAmount:F2}, Threshold: ${warningThreshold:F2}");
        await Task.Delay(100);
    }

    public async Task OnBudgetGoalReached(Guid budgetId, decimal targetAmount)
    {
        Console.WriteLine($"EMAIL SUCCESS: Budget goal reached for {budgetId}! Target: ${targetAmount:F2}");
        await Task.Delay(100);
    }
}