namespace Budgets.Application.DTOs;

public class BudgetCalculationRequest
{
    public Guid BudgetId { get; set; }
    public string StrategyType { get; set; } = "Fixed"; // "Fixed", "Rolling", "PercentageBased"
    public decimal CurrentSpending { get; set; }
}

public class BudgetCalculationResponse
{
    public decimal AvailableAmount { get; set; }
    public decimal TotalBudget { get; set; }
    public decimal CurrentSpending { get; set; }
    public string StrategyUsed { get; set; } = "";
    public decimal SpentPercentage { get; set; }
    public List<string> Warnings { get; set; } = new();
}