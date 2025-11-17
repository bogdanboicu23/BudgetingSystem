using ExpenseService.Domain.ValueObjects;
using ExpenseService.Domain.DomainEvents;

namespace ExpenseService.Domain.Entities;

public class Expense
{
    public ExpenseId Id { get; private set; }
    public UserId UserId { get; private set; }
    public BudgetId? BudgetId { get; private set; }
    public string Description { get; private set; }
    public Money Amount { get; private set; }
    public ExpenseCategory Category { get; private set; }
    public DateTime Date { get; private set; }
    public string? Merchant { get; private set; }
    public string? Notes { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private readonly List<IDomainEvent> _domainEvents = new();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    private Expense() { } // For EF Core

    public Expense(UserId userId, string description, Money amount, ExpenseCategory category, DateTime date, BudgetId? budgetId = null)
    {
        Id = ExpenseId.New();
        UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        Amount = amount ?? throw new ArgumentNullException(nameof(amount));
        Category = category;
        Date = date;
        BudgetId = budgetId;
        CreatedAt = DateTime.UtcNow;

        _domainEvents.Add(new ExpenseCreatedEvent(Id, UserId, Amount, Category, Date, BudgetId));
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty", nameof(description));

        Description = description;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateAmount(Money amount)
    {
        if (amount.Amount <= 0)
            throw new ArgumentException("Amount must be positive", nameof(amount));

        var oldAmount = Amount;
        Amount = amount;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new ExpenseAmountUpdatedEvent(Id, oldAmount, Amount));
    }

    public void UpdateCategory(ExpenseCategory category)
    {
        var oldCategory = Category;
        Category = category;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new ExpenseCategoryUpdatedEvent(Id, oldCategory, Category));
    }

    public void AssignToBudget(BudgetId budgetId)
    {
        BudgetId = budgetId;
        LastModifiedAt = DateTime.UtcNow;

        _domainEvents.Add(new ExpenseAssignedToBudgetEvent(Id, budgetId, Amount));
    }

    public void AddMerchant(string merchant)
    {
        Merchant = merchant;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void AddNotes(string notes)
    {
        Notes = notes;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}