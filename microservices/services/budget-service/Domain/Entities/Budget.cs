using BudgetService.Domain.ValueObjects;
using BudgetService.Domain.DomainEvents;

namespace BudgetService.Domain.Entities;

public class Budget
{
    public BudgetId Id { get; private set; }
    public UserId UserId { get; private set; }
    public string Name { get; private set; }
    public Money TotalAmount { get; private set; }
    public Money SpentAmount { get; private set; }
    public BudgetPeriod Period { get; private set; }
    public BudgetStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private readonly List<BudgetCategory> _categories = new();
    public IReadOnlyList<BudgetCategory> Categories => _categories.AsReadOnly();

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Budget() { } // For EF Core

    public Budget(UserId userId, string name, Money totalAmount, BudgetPeriod period)
    {
        Id = BudgetId.New();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount));
        SpentAmount = Money.Zero(totalAmount.Currency);
        Period = period ?? throw new ArgumentNullException(nameof(period));
        Status = BudgetStatus.Active;
        CreatedAt = DateTime.UtcNow;

        _domainEvents.Add(new BudgetCreatedEvent(Id, UserId, Name, TotalAmount));
    }

    public void AddCategory(string categoryName, Money allocatedAmount)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

        if (_categories.Any(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException($"Category '{categoryName}' already exists");

        var totalAllocated = _categories.Sum(c => c.AllocatedAmount.Amount) + allocatedAmount.Amount;
        if (totalAllocated > TotalAmount.Amount)
            throw new InvalidOperationException("Total allocated amount cannot exceed budget total");

        var category = new BudgetCategory(categoryName, allocatedAmount);
        _categories.Add(category);
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new BudgetCategoryAddedEvent(Id, categoryName, allocatedAmount));
    }

    public void RecordExpense(Money amount, string categoryName = null)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Expense amount must be positive", nameof(amount));

        if (amount.Currency != TotalAmount.Currency)
            throw new ArgumentException("Currency mismatch", nameof(amount));

        var newSpentAmount = Money.Add(SpentAmount, amount);

        if (!string.IsNullOrEmpty(categoryName))
        {
            var category = _categories.FirstOrDefault(c => c.Name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
            if (category == null)
                throw new InvalidOperationException($"Category '{categoryName}' not found");

            category.RecordExpense(amount);
        }

        SpentAmount = newSpentAmount;
        LastModifiedAt = DateTime.UtcNow;

        if (SpentAmount.Amount > TotalAmount.Amount)
        {
            Status = BudgetStatus.Exceeded;
            _domainEvents.Add(new BudgetExceededEvent(Id, SpentAmount, TotalAmount));
        }

        _domainEvents.Add(new BudgetExpenseRecordedEvent(Id, amount, categoryName));
    }

    public void UpdateBudgetAmount(Money newAmount)
    {
        if (newAmount.Amount <= 0)
            throw new ArgumentException("Budget amount must be positive", nameof(newAmount));

        TotalAmount = newAmount;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new BudgetAmountUpdatedEvent(Id, newAmount));
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}