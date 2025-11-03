using Monolith_BudgetSystem.Shared.Entities;

namespace Expenses.Domain.Entities;

public class Expense : BaseEntity
{
    #region Properties
    public Guid UserId { get; private set; }
    public Guid BudgetCategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Vendor { get; private set; }

    #endregion
    
    #region Constructors
    public Expense(Guid userId, Guid budgetCategoryId, decimal amount, DateTime date, string vendor)
    {
        UserId = userId;
        BudgetCategoryId = budgetCategoryId;
        Amount = amount;
        Date = date;
        Vendor = vendor;
    }
    #endregion
}