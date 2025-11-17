namespace Budgets.Domain.Decorators;

public class PlannedBudgetDecorator : BudgetDecorator
{
    private readonly decimal _plannedAdjustment;
    private readonly string _plannedNote;

    public PlannedBudgetDecorator(IBudgetComponent component, decimal plannedAdjustment = 0, string plannedNote = "")
        : base(component)
    {
        _plannedAdjustment = plannedAdjustment;
        _plannedNote = plannedNote;
    }

    public override decimal GetPlannedAmount()
    {
        return _component.GetPlannedAmount() + _plannedAdjustment;
    }

    public override decimal GetRemainingAmount()
    {
        return Math.Max(0, GetPlannedAmount() - GetSpentAmount());
    }

    public override string GetDisplayInfo()
    {
        var baseInfo = _component.GetDisplayInfo();
        var plannedInfo = $"Planned: ${GetPlannedAmount():F2}";

        if (!string.IsNullOrEmpty(_plannedNote))
        {
            plannedInfo += $" ({_plannedNote})";
        }

        return $"{baseInfo} | {plannedInfo}";
    }

    public override BudgetStatus GetStatus()
    {
        var spentPercentage = GetSpentAmount() / GetPlannedAmount();

        return spentPercentage switch
        {
            >= 1.0m => BudgetStatus.OverBudget,
            >= 0.9m => BudgetStatus.Warning,
            >= 0.8m => BudgetStatus.OnTrack,
            _ => BudgetStatus.OnTrack
        };
    }

    public string GetPlannedNote()
    {
        return _plannedNote;
    }

    public decimal GetPlannedAdjustment()
    {
        return _plannedAdjustment;
    }
}