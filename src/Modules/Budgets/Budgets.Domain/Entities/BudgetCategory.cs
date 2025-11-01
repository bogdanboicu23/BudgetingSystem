using System.Transactions;
using Monolith_BudgetSystem.Shared.Entities;
using Transactions.Domain.Enums;
using Transaction = Transactions.Domain.Entities.Transaction;

namespace Budgets.Domain.Entities;

public class BudgetCategory : BaseEntity
{
    #region Properties
    public string Name { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount => PlannedAmount - SpentAmount;

    #endregion
    
    #region Constructors
    public BudgetCategory(string name, decimal plannedAmount)
    {
        Name = name;
        PlannedAmount = plannedAmount;
    }

    #endregion
    
    #region Methods
    public void RegisterTransaction(Transaction transaction)
    {
        if (transaction.Type == TransactionType.Expense)
            SpentAmount += transaction.Amount;
        if(transaction.Type == TransactionType.Income) 
            PlannedAmount += transaction.Amount;
    }
    
    #endregion
}