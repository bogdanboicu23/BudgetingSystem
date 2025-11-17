using System.Transactions;
using Budgets.Domain.Decorators;
using Budgets.Domain.Strategies;
using Monolith_BudgetSystem.Shared.Entities;
using Transaction = Transactions.Domain.Entities.Transaction;

namespace Budgets.Domain.Entities;

public class Budget : BaseEntity
{
    #region Properties
    public new Guid Id { get; private set; }
    public new string Name { get; private set; }
    public DateTime PeriodStart { get; private set; }
    public DateTime PeriodEnd { get; private set; }
    public string Category => "General"; // Default category for the budget
    public BudgetStatus Status { get; private set; }

    // Planned vs Actual tracking
    private readonly List<PlannedBudgetItem> _plannedIncomes = new();
    private readonly List<PlannedBudgetItem> _plannedExpenses = new();

    public IReadOnlyList<PlannedBudgetItem> PlannedIncomes => _plannedIncomes.AsReadOnly();
    public IReadOnlyList<PlannedBudgetItem> PlannedExpenses => _plannedExpenses.AsReadOnly();

    // Calculated properties for decorator pattern
    public decimal Amount => _plannedExpenses.Sum(e => e.PlannedAmount);
    public decimal SpentAmount => _plannedExpenses.Sum(e => e.LinkedAmount);
    public decimal PlannedIncomeAmount => _plannedIncomes.Sum(i => i.PlannedAmount);
    public decimal ActualIncomeAmount => _plannedIncomes.Sum(i => i.LinkedAmount);

    // Legacy support for existing categories
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

    public decimal CalculateAvailableAmount(IBudgetCalculationStrategy strategy, decimal spentAmount)
    {
        var totalBudget = _budgetCategories.Sum(c => c.PlannedAmount);
        return strategy.CalculateAvailableAmount(totalBudget, spentAmount, PeriodStart, PeriodEnd);
    }

    public decimal GetTotalBudgeted()
    {
        return _budgetCategories.Sum(c => c.PlannedAmount);
    }

    // New methods for planned vs actual flow
    public void AddPlannedIncome(
        string name,
        string description,
        decimal plannedAmount,
        string category,
        DateTime? dueDate = null,
        bool isRecurring = false,
        RecurrenceFrequency? frequency = null)
    {
        var plannedItem = new PlannedBudgetItem(
            name, description, plannedAmount, category,
            PlannedItemType.Income, dueDate, isRecurring, frequency);

        _plannedIncomes.Add(plannedItem);
        UpdateBudgetStatus();
    }

    public void AddPlannedExpense(
        string name,
        string description,
        decimal plannedAmount,
        string category,
        DateTime? dueDate = null,
        bool isRecurring = false,
        RecurrenceFrequency? frequency = null)
    {
        var plannedItem = new PlannedBudgetItem(
            name, description, plannedAmount, category,
            PlannedItemType.Expense, dueDate, isRecurring, frequency);

        _plannedExpenses.Add(plannedItem);
        UpdateBudgetStatus();
    }

    public void LinkTransactionToPlannedItem(Guid plannedItemId, Guid transactionId, decimal amount)
    {
        var plannedItem = _plannedIncomes.FirstOrDefault(i => i.Id == plannedItemId) ??
                         _plannedExpenses.FirstOrDefault(e => e.Id == plannedItemId);

        if (plannedItem != null)
        {
            plannedItem.LinkTransaction(transactionId, amount);
            UpdateBudgetStatus();
        }
    }

    public void UnlinkTransactionFromPlannedItem(Guid plannedItemId, Guid transactionId, decimal amount)
    {
        var plannedItem = _plannedIncomes.FirstOrDefault(i => i.Id == plannedItemId) ??
                         _plannedExpenses.FirstOrDefault(e => e.Id == plannedItemId);

        if (plannedItem != null)
        {
            plannedItem.UnlinkTransaction(transactionId, amount);
            UpdateBudgetStatus();
        }
    }

    public decimal GetRemainingBudget()
    {
        return Amount - SpentAmount;
    }

    public decimal GetIncomeVariance()
    {
        return ActualIncomeAmount - PlannedIncomeAmount;
    }

    public decimal GetExpenseVariance()
    {
        return SpentAmount - Amount;
    }

    public BudgetPerformanceSummary GetPerformanceSummary()
    {
        var timeElapsed = DateTime.Now - PeriodStart;
        var totalPeriod = PeriodEnd - PeriodStart;
        var timeProgress = totalPeriod.TotalDays > 0 ? timeElapsed.TotalDays / totalPeriod.TotalDays : 0;

        return new BudgetPerformanceSummary
        {
            TimeProgress = Math.Min(1.0, Math.Max(0.0, timeProgress)),
            SpendingProgress = Amount > 0 ? (double)(SpentAmount / Amount) : 0,
            IncomeProgress = PlannedIncomeAmount > 0 ? (double)(ActualIncomeAmount / PlannedIncomeAmount) : 0,
            RemainingBudget = GetRemainingBudget(),
            IncomeVariance = GetIncomeVariance(),
            ExpenseVariance = GetExpenseVariance(),
            Status = Status
        };
    }

    private void UpdateBudgetStatus()
    {
        var remainingBudget = GetRemainingBudget();
        var spentPercentage = Amount > 0 ? SpentAmount / Amount : 0;

        Status = spentPercentage switch
        {
            >= 1.0m => BudgetStatus.OverBudget,
            >= 0.9m => BudgetStatus.Warning,
            _ => BudgetStatus.OnTrack
        };
    }
    #endregion
}