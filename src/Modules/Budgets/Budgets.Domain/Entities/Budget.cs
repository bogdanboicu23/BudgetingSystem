using System.Transactions;
using Monolith_BudgetSystem.Shared.Entities;
using Transaction = Transactions.Domain.Entities.Transaction;

namespace Budgets.Domain.Entities;

public class Budget : BaseEntity
{
    #region Properties
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    
    private readonly List<BudgetCategory> _budgetCategories = new();
    public IReadOnlyCollection<BudgetCategory> BudgetCategories => _budgetCategories.AsReadOnly();

    #endregion
    
    #region Constructors
    public Budget(Guid id, string name, DateTime periodStart, DateTime periodEnd)
    {
        Name = name;
        Id = id;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
        
    }

    #endregion
    
    #region Methods
    public void AddCategory(string name, decimal plannedAmount)
    {
        _budgetCategories.Add(new BudgetCategory(name, plannedAmount));
    }

    public void ApplyTransaction(Transaction transaction)
    {
        var category = _budgetCategories.FirstOrDefault(c => c.Id == transaction.CategoryId);
        if (category != null)
        {
            category.RegisterTransaction(transaction);
        }
    }
    #endregion
}