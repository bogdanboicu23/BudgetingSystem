using Budgets.Application.Reports;
using Budgets.Domain.Entities;
using Budgets.Domain.Events;
using Budgets.Domain.Strategies;

namespace Budgets.Application.Services;

public class BudgetService
{
    private readonly BudgetSubject _budgetSubject;

    public BudgetService(BudgetSubject budgetSubject)
    {
        _budgetSubject = budgetSubject;
    }

    public async Task<decimal> CalculateBudgetAvailableAmountAsync(
        Budget budget,
        IBudgetCalculationStrategy strategy,
        decimal currentSpending)
    {
        var availableAmount = budget.CalculateAvailableAmount(strategy, currentSpending);

        var totalBudget = budget.GetTotalBudgeted();
        var spentPercentage = currentSpending / totalBudget;

        if (spentPercentage >= 1.0m)
        {
            await _budgetSubject.NotifyBudgetLimitExceeded(budget.Id, currentSpending, totalBudget);
        }
        else if (spentPercentage >= 0.8m)
        {
            await _budgetSubject.NotifyBudgetWarning(budget.Id, currentSpending, totalBudget * 0.8m);
        }

        return availableAmount;
    }

    public async Task<string> GenerateBudgetReportAsync(
        AbstractBudgetReportGenerator reportGenerator,
        Guid userId,
        DateTime startDate,
        DateTime endDate)
    {
        return await reportGenerator.GenerateReportAsync(userId, startDate, endDate);
    }
}