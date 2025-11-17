namespace Budgets.Domain.Events;

public class BudgetSubject
{
    private readonly List<IBudgetObserver> _observers = new();

    public void Subscribe(IBudgetObserver observer)
    {
        _observers.Add(observer);
    }

    public void Unsubscribe(IBudgetObserver observer)
    {
        _observers.Remove(observer);
    }

    public async Task NotifyBudgetLimitExceeded(Guid budgetId, decimal currentAmount, decimal limitAmount)
    {
        var tasks = _observers.Select(observer =>
            observer.OnBudgetLimitExceeded(budgetId, currentAmount, limitAmount));

        await Task.WhenAll(tasks);
    }

    public async Task NotifyBudgetWarning(Guid budgetId, decimal currentAmount, decimal warningThreshold)
    {
        var tasks = _observers.Select(observer =>
            observer.OnBudgetWarningThreshold(budgetId, currentAmount, warningThreshold));

        await Task.WhenAll(tasks);
    }

    public async Task NotifyBudgetGoalReached(Guid budgetId, decimal targetAmount)
    {
        var tasks = _observers.Select(observer =>
            observer.OnBudgetGoalReached(budgetId, targetAmount));

        await Task.WhenAll(tasks);
    }
}