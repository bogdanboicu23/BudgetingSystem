using BudgetingSystem.Budgets.Domain.Entities;
using BudgetingSystem.Shared.Domain.Interfaces;

namespace BudgetingSystem.Budgets.Application.Services;

public class BudgetService
{
    private readonly IRepository<Budget> _budgetRepository;

    public BudgetService(IRepository<Budget> budgetRepository)
    {
        _budgetRepository = budgetRepository;
    }

    public async Task<Budget> CreateBudgetAsync(Guid userId, string name, string description, decimal totalAmount, DateTime startDate, DateTime endDate)
    {
        var budget = new Budget
        {
            UserId = userId,
            Name = name,
            Description = description,
            TotalAmount = totalAmount,
            StartDate = startDate,
            EndDate = endDate,
            Status = BudgetStatus.Active
        };

        return await _budgetRepository.AddAsync(budget);
    }

    public async Task<IEnumerable<Budget>> GetUserBudgetsAsync(Guid userId)
    {
        var budgets = await _budgetRepository.GetAllAsync();
        return budgets.Where(b => b.UserId == userId);
    }

    public async Task<Budget?> GetBudgetByIdAsync(Guid budgetId)
    {
        return await _budgetRepository.GetByIdAsync(budgetId);
    }

    public async Task<Budget> UpdateBudgetSpentAmountAsync(Guid budgetId, decimal spentAmount)
    {
        var budget = await _budgetRepository.GetByIdAsync(budgetId);
        if (budget == null)
            throw new ArgumentException("Budget not found");

        budget.SpentAmount = spentAmount;
        budget.Status = spentAmount > budget.TotalAmount ? BudgetStatus.Exceeded : BudgetStatus.Active;
        budget.UpdatedAt = DateTime.UtcNow;

        return await _budgetRepository.UpdateAsync(budget);
    }
}