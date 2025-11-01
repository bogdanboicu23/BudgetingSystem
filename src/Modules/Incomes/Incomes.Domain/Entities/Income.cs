using Monolith_BudgetSystem.Shared.Entities;

namespace Incomes.Domain.Entities;

public class Income : BaseEntity
{
    #region Properties
    public Guid UserId { get; private set; }
    public Guid? BudgetCategoryId { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Source { get; private set; }

    #endregion
    
    #region Constructors
    public Income(Guid userId, Guid budgetCategoryId, decimal amount, DateTime date, string source)
    {
        UserId = userId;
        BudgetCategoryId = budgetCategoryId;
        Amount = amount;
        Date = date;
        Source = source;
    }
    
    #endregion
}