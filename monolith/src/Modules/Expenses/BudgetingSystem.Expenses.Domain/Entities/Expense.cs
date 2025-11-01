using BudgetingSystem.Shared.Domain.Entities;

namespace BudgetingSystem.Expenses.Domain.Entities;

public class Expense : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid? BudgetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public string? Notes { get; set; }
}