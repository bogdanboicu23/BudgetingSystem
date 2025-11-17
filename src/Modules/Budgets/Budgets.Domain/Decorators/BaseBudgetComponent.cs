using Budgets.Domain.Entities;

namespace Budgets.Domain.Decorators;

public class BaseBudgetComponent : IBudgetComponent
{
    private readonly Budget _budget;

    public BaseBudgetComponent(Budget budget)
    {
        _budget = budget;
    }

    public Guid Id => _budget.Id;
    public string Name => _budget.Name;
    public decimal Amount => _budget.Amount;
    public DateTime PeriodStart => _budget.PeriodStart;
    public DateTime PeriodEnd => _budget.PeriodEnd;
    public string Category => _budget.Category;

    public virtual decimal GetTotalAmount()
    {
        return _budget.Amount;
    }

    public virtual decimal GetSpentAmount()
    {
        return _budget.SpentAmount;
    }

    public virtual decimal GetRemainingAmount()
    {
        return Math.Max(0, _budget.Amount - _budget.SpentAmount);
    }

    public virtual decimal GetPlannedAmount()
    {
        return _budget.Amount;
    }

    public virtual string GetDisplayInfo()
    {
        return $"{_budget.Name}: ${GetTotalAmount():F2} ({_budget.Category})";
    }

    public virtual BudgetStatus GetStatus()
    {
        var spentPercentage = GetSpentAmount() / GetTotalAmount();

        return spentPercentage switch
        {
            >= 1.0m => BudgetStatus.OverBudget,
            >= 0.9m => BudgetStatus.Warning,
            >= 0.8m => BudgetStatus.OnTrack,
            _ => BudgetStatus.OnTrack
        };
    }
}