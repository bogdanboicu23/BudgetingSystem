namespace Expenses.Domain.Approval;

public class BudgetAvailabilityHandler : BaseApprovalHandler
{
    protected override async Task<ApprovalResult> ProcessRequest(ExpenseApprovalRequest request)
    {
        await Task.Delay(20); // Simulate budget check

        // Simulate checking if budget category has enough remaining funds
        var hasAvailableBudget = request.Metadata.TryGetValue("AvailableBudget", out var budgetObj)
            && budgetObj is decimal availableBudget
            && availableBudget >= request.Expense.Amount;

        if (hasAvailableBudget)
        {
            return new ApprovalResult
            {
                IsApproved = true,
                HandlerName = nameof(BudgetAvailabilityHandler),
                Reason = "Sufficient budget available for this expense"
            };
        }

        return new ApprovalResult
        {
            IsApproved = false,
            HandlerName = nameof(BudgetAvailabilityHandler),
            Reason = "Insufficient budget available for this expense"
        };
    }
}