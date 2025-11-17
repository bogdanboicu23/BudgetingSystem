namespace Expenses.Domain.Approval;

public class PersonalLimitHandler : BaseApprovalHandler
{
    private readonly decimal _dailyLimit;
    private readonly decimal _monthlyLimit;

    public PersonalLimitHandler(decimal dailyLimit = 200m, decimal monthlyLimit = 2000m)
    {
        _dailyLimit = dailyLimit;
        _monthlyLimit = monthlyLimit;
    }

    protected override async Task<ApprovalResult> ProcessRequest(ExpenseApprovalRequest request)
    {
        await Task.Delay(20);

        // Check daily spending limit
        var todaySpending = request.Metadata.TryGetValue("TodaySpending", out var todayObj)
            ? (decimal)(todayObj ?? 0m)
            : 0m;

        if (todaySpending + request.Expense.Amount > _dailyLimit)
        {
            return new ApprovalResult
            {
                IsApproved = false,
                HandlerName = nameof(PersonalLimitHandler),
                Reason = $"Daily spending limit exceeded. Today: ${todaySpending:F2}, Limit: ${_dailyLimit:F2}"
            };
        }

        // Check monthly spending limit
        var monthlySpending = request.Metadata.TryGetValue("MonthlySpending", out var monthlyObj)
            ? (decimal)(monthlyObj ?? 0m)
            : 0m;

        if (monthlySpending + request.Expense.Amount > _monthlyLimit)
        {
            return new ApprovalResult
            {
                IsApproved = false,
                HandlerName = nameof(PersonalLimitHandler),
                Reason = $"Monthly spending limit exceeded. This month: ${monthlySpending:F2}, Limit: ${_monthlyLimit:F2}"
            };
        }

        return new ApprovalResult
        {
            IsApproved = true,
            HandlerName = nameof(PersonalLimitHandler),
            Reason = "Within personal daily and monthly spending limits"
        };
    }
}