using Monolith_BudgetSystem.Shared.Entities;

namespace Budgets.Domain.Entities;

public class PlannedBudgetItem : BaseEntity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal PlannedAmount { get; private set; }
    public string Category { get; private set; }
    public PlannedItemType Type { get; private set; }
    public DateTime? DueDate { get; private set; }
    public bool IsRecurring { get; private set; }
    public RecurrenceFrequency? Frequency { get; private set; }

    // Tracking linked transactions
    private readonly List<Guid> _linkedTransactionIds = new();
    public IReadOnlyList<Guid> LinkedTransactionIds => _linkedTransactionIds.AsReadOnly();

    public decimal LinkedAmount { get; private set; }
    public decimal RemainingAmount => PlannedAmount - LinkedAmount;

    public PlannedBudgetItem(
        string name,
        string description,
        decimal plannedAmount,
        string category,
        PlannedItemType type,
        DateTime? dueDate = null,
        bool isRecurring = false,
        RecurrenceFrequency? frequency = null)
    {
        Name = name;
        Description = description;
        PlannedAmount = plannedAmount;
        Category = category;
        Type = type;
        DueDate = dueDate;
        IsRecurring = isRecurring;
        Frequency = frequency;
        LinkedAmount = 0;
    }

    public void LinkTransaction(Guid transactionId, decimal amount)
    {
        if (!_linkedTransactionIds.Contains(transactionId))
        {
            _linkedTransactionIds.Add(transactionId);
            LinkedAmount += amount;
        }
    }

    public void UnlinkTransaction(Guid transactionId, decimal amount)
    {
        if (_linkedTransactionIds.Contains(transactionId))
        {
            _linkedTransactionIds.Remove(transactionId);
            LinkedAmount -= amount;
        }
    }

    public void UpdatePlannedAmount(decimal newAmount)
    {
        PlannedAmount = newAmount;
    }

    public decimal GetUtilizationPercentage()
    {
        return PlannedAmount == 0 ? 0 : (LinkedAmount / PlannedAmount) * 100;
    }

    public bool IsOverBudget()
    {
        return LinkedAmount > PlannedAmount;
    }

    public bool IsCompleted()
    {
        return Math.Abs(LinkedAmount - PlannedAmount) < 0.01m; // Account for decimal precision
    }
}

public enum PlannedItemType
{
    Income,
    Expense
}

public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Annually
}