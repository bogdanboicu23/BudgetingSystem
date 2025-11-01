using BudgetingSystem.Shared.Domain.Entities;

namespace BudgetingSystem.Budgets.Domain.Entities;

public class Budget : BaseEntity
{
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public BudgetStatus Status { get; set; } = BudgetStatus.Active;

    public decimal RemainingAmount => TotalAmount - SpentAmount;
}

public enum BudgetStatus
{
    Active,
    Completed,
    Exceeded,
    Inactive
}