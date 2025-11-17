namespace Budgets.Domain.Decorators;

public class SpentTrackingDecorator : BudgetDecorator
{
    private readonly List<SpentTransaction> _transactions;
    private readonly bool _includeProjectedSpending;

    public SpentTrackingDecorator(IBudgetComponent component, bool includeProjectedSpending = false)
        : base(component)
    {
        _transactions = new List<SpentTransaction>();
        _includeProjectedSpending = includeProjectedSpending;
    }

    public override decimal GetSpentAmount()
    {
        var actualSpent = _component.GetSpentAmount() + _transactions.Sum(t => t.Amount);

        if (_includeProjectedSpending)
        {
            return actualSpent + GetProjectedSpending();
        }

        return actualSpent;
    }

    public override decimal GetRemainingAmount()
    {
        return Math.Max(0, GetTotalAmount() - GetSpentAmount());
    }

    public override string GetDisplayInfo()
    {
        var baseInfo = _component.GetDisplayInfo();
        var spentInfo = $"Spent: ${GetSpentAmount():F2}";
        var remainingInfo = $"Remaining: ${GetRemainingAmount():F2}";

        if (_includeProjectedSpending)
        {
            spentInfo += $" (incl. projected: ${GetProjectedSpending():F2})";
        }

        return $"{baseInfo} | {spentInfo} | {remainingInfo}";
    }

    public override BudgetStatus GetStatus()
    {
        var remainingPercentage = GetRemainingAmount() / GetTotalAmount();

        return remainingPercentage switch
        {
            <= 0m => BudgetStatus.OverBudget,
            <= 0.1m => BudgetStatus.Warning,
            _ => BudgetStatus.OnTrack
        };
    }

    public void AddTransaction(decimal amount, string description, DateTime date)
    {
        _transactions.Add(new SpentTransaction
        {
            Amount = amount,
            Description = description,
            Date = date
        });
    }

    public IReadOnlyList<SpentTransaction> GetTransactions()
    {
        return _transactions.AsReadOnly();
    }

    private decimal GetProjectedSpending()
    {
        // Calculate projected spending based on current spending rate
        var daysInPeriod = (PeriodEnd - PeriodStart).Days;
        var daysElapsed = (DateTime.Now - PeriodStart).Days;

        if (daysElapsed <= 0) return 0;

        var dailySpendingRate = _component.GetSpentAmount() / daysElapsed;
        var remainingDays = (PeriodEnd - DateTime.Now).Days;

        return remainingDays > 0 ? dailySpendingRate * remainingDays : 0;
    }

    public decimal GetProjectedTotal()
    {
        return _component.GetSpentAmount() + GetProjectedSpending();
    }
}

public class SpentTransaction
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Date { get; set; }
}