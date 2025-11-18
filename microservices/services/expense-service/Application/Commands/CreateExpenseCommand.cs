using ExpenseService.Domain.ValueObjects;

namespace ExpenseService.Application.Commands;

public class CreateExpenseCommand
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