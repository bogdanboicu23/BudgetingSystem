namespace Expenses.Domain.Approval;

public class AmountLimitHandler : BaseApprovalHandler
{
    private readonly decimal _autoApprovalLimit;

    public AmountLimitHandler(decimal autoApprovalLimit = 100m)
    {
        _autoApprovalLimit = autoApprovalLimit;
    }

    protected override async Task<ApprovalResult> ProcessRequest(ExpenseApprovalRequest request)
    {
        await Task.Delay(10); // Simulate processing time

        if (request.Expense.Amount <= _autoApprovalLimit)
        {
            return new ApprovalResult
            {
                IsApproved = true,
                HandlerName = nameof(AmountLimitHandler),
                Reason = $"Auto-approved: Amount ${request.Expense.Amount:F2} is within limit ${_autoApprovalLimit:F2}"
            };
        }

        return new ApprovalResult
        {
            IsApproved = false,
            HandlerName = nameof(AmountLimitHandler),
            Reason = $"Amount ${request.Expense.Amount:F2} exceeds auto-approval limit ${_autoApprovalLimit:F2}"
        };
    }
}