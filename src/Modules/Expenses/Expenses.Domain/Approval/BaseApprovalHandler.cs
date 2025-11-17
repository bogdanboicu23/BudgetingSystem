namespace Expenses.Domain.Approval;

public abstract class BaseApprovalHandler : IExpenseApprovalHandler
{
    private IExpenseApprovalHandler _nextHandler;

    public IExpenseApprovalHandler SetNext(IExpenseApprovalHandler handler)
    {
        _nextHandler = handler;
        return handler;
    }

    public virtual async Task<ApprovalResult> Handle(ExpenseApprovalRequest request)
    {
        var result = await ProcessRequest(request);

        if (!result.IsApproved && _nextHandler != null)
        {
            return await _nextHandler.Handle(request);
        }

        return result;
    }

    protected abstract Task<ApprovalResult> ProcessRequest(ExpenseApprovalRequest request);
}