using Budgets.Domain.Entities;

namespace Budgets.Domain.Decorators;

public interface IBudgetComponent
{
    Guid Id { get; }
    string Name { get; }
    decimal Amount { get; }
    DateTime PeriodStart { get; }
    DateTime PeriodEnd { get; }
    string Category { get; }

    decimal GetTotalAmount();
    decimal GetSpentAmount();
    decimal GetRemainingAmount();
    decimal GetPlannedAmount();
    string GetDisplayInfo();
    BudgetStatus GetStatus();
}

public enum BudgetStatus
{
    OnTrack,
    Warning,
    OverBudget,
    Completed
}