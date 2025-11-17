using BudgetService.Domain.ValueObjects;

namespace BudgetService.Domain.Entities;

public class BudgetCategory
{
    public string Name { get; private set; }
    public Money AllocatedAmount { get; private set; }
    public Money SpentAmount { get; private set; }
    public Money RemainingAmount => Money.Subtract(AllocatedAmount, SpentAmount);
    public decimal UtilizationPercentage => AllocatedAmount.Amount == 0 ? 0 : (SpentAmount.Amount / AllocatedAmount.Amount) * 100;

    private BudgetCategory() { } // For EF Core

    public BudgetCategory(string name, Money allocatedAmount)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AllocatedAmount = allocatedAmount ?? throw new ArgumentNullException(nameof(allocatedAmount));
        SpentAmount = Money.Zero(allocatedAmount.Currency);
    }

    public void RecordExpense(Money amount)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Expense amount must be positive", nameof(amount));

        if (amount.Currency != AllocatedAmount.Currency)
            throw new ArgumentException("Currency mismatch", nameof(amount));

        SpentAmount = Money.Add(SpentAmount, amount);
    }

    public void UpdateAllocation(Money newAmount)
    {
        if (newAmount.Amount < SpentAmount.Amount)
            throw new InvalidOperationException("Allocated amount cannot be less than already spent amount");

        AllocatedAmount = newAmount;
    }

    public bool IsOverBudget => SpentAmount.Amount > AllocatedAmount.Amount;
}