using Budgets.Domain.Decorators;
using MediatR;

namespace Budgets.Application.Commands.EnhanceBudget;

public class EnhanceBudgetCommand : IRequest<EnhanceBudgetResponse>
{
    public Guid BudgetId { get; set; }
    public bool EnablePlannedTracking { get; set; } = true;
    public decimal PlannedAdjustment { get; set; } = 0;
    public string PlannedNote { get; set; } = "";
    public bool EnableSpentTracking { get; set; } = true;
    public bool IncludeProjectedSpending { get; set; } = false;
    public bool EnableRemainingAnalysis { get; set; } = true;
    public decimal ReserveAmount { get; set; } = 0;
    public bool EnableSmartAlerts { get; set; } = true;
}

public class EnhanceBudgetResponse
{
    public Guid BudgetId { get; set; }
    public string BudgetName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal PlannedAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public BudgetStatus Status { get; set; }
    public string DisplayInfo { get; set; } = string.Empty;
    public List<string> AppliedEnhancements { get; set; } = new();
    public List<BudgetAlertDto> Alerts { get; set; } = new();
}

public class BudgetAlertDto
{
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}