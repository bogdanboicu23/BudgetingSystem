using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Application.DTOs;

public class ExpenseDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid? BudgetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public ExpenseCategory Category { get; set; }
    public DateTime Date { get; set; }
    public string? Merchant { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastModifiedAt { get; set; }
}

public class CreateExpenseDto
{
    public Guid UserId { get; set; }
    public Guid? BudgetId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public ExpenseCategory Category { get; set; }
    public DateTime Date { get; set; }
    public string? Merchant { get; set; }
    public string? Notes { get; set; }
}

public class UpdateExpenseDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public ExpenseCategory Category { get; set; }
    public string? Merchant { get; set; }
    public string? Notes { get; set; }
}