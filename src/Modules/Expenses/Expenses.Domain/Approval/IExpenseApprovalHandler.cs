using Expenses.Domain.Entities;

namespace Expenses.Domain.Approval;

public interface IExpenseApprovalHandler
{
    IExpenseApprovalHandler SetNext(IExpenseApprovalHandler handler);
    Task<ApprovalResult> Handle(ExpenseApprovalRequest request);
}

public class ExpenseApprovalRequest
{
    public Expense Expense { get; set; }
    public Guid UserId { get; set; }
    public string Reason { get; set; }
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public class ApprovalResult
{
    public bool IsApproved { get; set; }
    public string HandlerName { get; set; }
    public string Reason { get; set; }
    public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
}