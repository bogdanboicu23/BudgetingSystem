namespace Budgets.Domain.Decorators;

public class RemainingBudgetDecorator : BudgetDecorator
{
    private readonly decimal _reserveAmount;
    private readonly bool _enableSmartAlerts;

    public RemainingBudgetDecorator(IBudgetComponent component, decimal reserveAmount = 0, bool enableSmartAlerts = true)
        : base(component)
    {
        _reserveAmount = reserveAmount;
        _enableSmartAlerts = enableSmartAlerts;
    }

    public override decimal GetRemainingAmount()
    {
        var baseRemaining = _component.GetRemainingAmount();
        return Math.Max(0, baseRemaining - _reserveAmount);
    }

    public override string GetDisplayInfo()
    {
        var baseInfo = _component.GetDisplayInfo();
        var remainingInfo = $"Available: ${GetRemainingAmount():F2}";

        if (_reserveAmount > 0)
        {
            remainingInfo += $" (Reserve: ${_reserveAmount:F2})";
        }

        var alerts = GetSmartAlerts();
        if (_enableSmartAlerts && alerts.Any())
        {
            remainingInfo += $" | Alerts: {string.Join(", ", alerts)}";
        }

        return $"{baseInfo} | {remainingInfo}";
    }

    public override BudgetStatus GetStatus()
    {
        var totalRemaining = _component.GetRemainingAmount();
        var availableRemaining = GetRemainingAmount();

        if (availableRemaining <= 0)
        {
            return BudgetStatus.OverBudget;
        }

        if (totalRemaining <= _reserveAmount * 1.1m) // Within 110% of reserve
        {
            return BudgetStatus.Warning;
        }

        return BudgetStatus.OnTrack;
    }

    public decimal GetReserveAmount()
    {
        return _reserveAmount;
    }

    public decimal GetTotalRemainingWithReserve()
    {
        return _component.GetRemainingAmount();
    }

    public List<string> GetSmartAlerts()
    {
        var alerts = new List<string>();

        if (!_enableSmartAlerts) return alerts;

        var totalAmount = GetTotalAmount();
        var remainingPercentage = totalAmount > 0 ? GetRemainingAmount() / totalAmount : 0;
        var timeRemainingPercentage = GetTimeRemainingPercentage();

        // Alert if spending too fast relative to time remaining
        if (remainingPercentage < timeRemainingPercentage * 0.8m)
        {
            alerts.Add("Spending too fast");
        }

        // Alert if reserve is being touched
        if (_reserveAmount > 0 && _component.GetRemainingAmount() <= _reserveAmount * 1.2m)
        {
            alerts.Add("Approaching reserve");
        }

        // Alert for end-of-period recommendations
        if (timeRemainingPercentage < 0.2m && remainingPercentage > 0.3m)
        {
            alerts.Add("Consider reallocating surplus");
        }

        return alerts;
    }

    private decimal GetTimeRemainingPercentage()
    {
        var totalDays = (PeriodEnd - PeriodStart).TotalDays;
        var daysRemaining = (PeriodEnd - DateTime.Now).TotalDays;

        if (totalDays <= 0) return 0;

        return Math.Max(0, (decimal)(daysRemaining / totalDays));
    }

    public decimal GetDailyBudgetRemaining()
    {
        var daysRemaining = (PeriodEnd - DateTime.Now).Days;
        return daysRemaining > 0 ? GetRemainingAmount() / daysRemaining : 0;
    }

    public decimal GetRecommendedDailySpending()
    {
        var timeRemainingPercentage = GetTimeRemainingPercentage();
        var availableAmount = GetRemainingAmount();

        if (timeRemainingPercentage <= 0) return 0;

        return availableAmount * (decimal)timeRemainingPercentage;
    }
}