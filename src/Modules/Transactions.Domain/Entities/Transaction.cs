using Monolith_BudgetSystem.Shared.Entities;
using Transactions.Domain.Enums;

namespace Transactions.Domain.Entities;

public class Transaction : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid CategoryId { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public TransactionType Type { get; set; }
    public string Description { get; set; }

    public Transaction(Guid userId, Guid categoryId, decimal amount, DateTime date, TransactionType type,
        string description)
    {
        UserId = userId;
        CategoryId = categoryId;
        Amount = amount;
        Date = date;
        Type = type;
        Description = description;
        
    }
}