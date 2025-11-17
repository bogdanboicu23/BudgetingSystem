namespace BudgetService.Domain.ValueObjects;

public enum BudgetStatus
{
    Active = 1,
    Inactive = 2,
    Exceeded = 3,
    Completed = 4,
    Cancelled = 5
}