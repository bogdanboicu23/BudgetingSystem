using Budgets.Domain.Events;
using Expenses.Domain.Approval;
using Expenses.Domain.Entities;

namespace Expenses.Application.Services;

public class ExpenseProcessingService
{
    private readonly IExpenseApprovalHandler _approvalChain;
    private readonly BudgetSubject _budgetSubject;

    public ExpenseProcessingService(IExpenseApprovalHandler approvalChain, BudgetSubject budgetSubject)
    {
        _approvalChain = approvalChain;
        _budgetSubject = budgetSubject;
    }

    public async Task<ExpenseProcessingResult> ProcessExpenseAsync(Expense expense, Guid userId, Dictionary<string, object>? metadata = null)
    {
        metadata ??= new Dictionary<string, object>();

        var approvalRequest = new ExpenseApprovalRequest
        {
            Expense = expense,
            UserId = userId,
            Reason = "New expense submission",
            Metadata = metadata
        };

        var approvalResult = await _approvalChain.Handle(approvalRequest);

        if (approvalResult.IsApproved)
        {
            await CheckBudgetLimits(expense);
        }

        return new ExpenseProcessingResult
        {
            IsApproved = approvalResult.IsApproved,
            ProcessedBy = approvalResult.HandlerName,
            Reason = approvalResult.Reason,
            ProcessedAt = approvalResult.ProcessedAt
        };
    }

    private async Task CheckBudgetLimits(Expense expense)
    {
        var currentCategorySpending = 800m; // Mock - would query from database
        var categoryLimit = 1000m;
        var warningThreshold = categoryLimit * 0.8m;

        if (currentCategorySpending + expense.Amount > categoryLimit)
        {
            await _budgetSubject.NotifyBudgetLimitExceeded(
                expense.BudgetCategoryId,
                currentCategorySpending + expense.Amount,
                categoryLimit);
        }
        else if (currentCategorySpending + expense.Amount > warningThreshold)
        {
            await _budgetSubject.NotifyBudgetWarning(
                expense.BudgetCategoryId,
                currentCategorySpending + expense.Amount,
                warningThreshold);
        }
    }
}

public class ExpenseProcessingResult
{
    public bool IsApproved { get; set; }
    public string ProcessedBy { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime ProcessedAt { get; set; }
}