namespace Budgets.Domain.Decorators;

public abstract class BudgetDecorator : IBudgetComponent
{
    protected readonly IBudgetComponent _component;

    protected BudgetDecorator(IBudgetComponent component)
    {
        _component = component;
    }

    public virtual Guid Id => _component.Id;
    public virtual string Name => _component.Name;
    public virtual decimal Amount => _component.Amount;
    public virtual DateTime PeriodStart => _component.PeriodStart;
    public virtual DateTime PeriodEnd => _component.PeriodEnd;
    public virtual string Category => _component.Category;

    public virtual decimal GetTotalAmount()
    {
        return _component.GetTotalAmount();
    }

    public virtual decimal GetSpentAmount()
    {
        return _component.GetSpentAmount();
    }

    public virtual decimal GetRemainingAmount()
    {
        return _component.GetRemainingAmount();
    }

    public virtual decimal GetPlannedAmount()
    {
        return _component.GetPlannedAmount();
    }

    public virtual string GetDisplayInfo()
    {
        return _component.GetDisplayInfo();
    }

    public virtual BudgetStatus GetStatus()
    {
        return _component.GetStatus();
    }
}