namespace BudgetService.Application.DTOs;

public record BudgetDto
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public decimal SpentAmount { get; init; }
    public decimal RemainingAmount => TotalAmount - SpentAmount;
    public string Currency { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public string PeriodType { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastModifiedAt { get; init; }
    public List<BudgetCategoryDto> Categories { get; init; } = new();
    public decimal UtilizationPercentage => TotalAmount == 0 ? 0 : (SpentAmount / TotalAmount) * 100;
    public bool IsOverBudget => SpentAmount > TotalAmount;
    public int DaysRemaining => EndDate > DateTime.UtcNow ? (EndDate - DateTime.UtcNow).Days : 0;
}

public record BudgetCategoryDto
{
    public string Name { get; init; } = string.Empty;
    public decimal AllocatedAmount { get; init; }
    public decimal SpentAmount { get; init; }
    public decimal RemainingAmount => AllocatedAmount - SpentAmount;
    public string Currency { get; init; } = string.Empty;
    public decimal UtilizationPercentage => AllocatedAmount == 0 ? 0 : (SpentAmount / AllocatedAmount) * 100;
    public bool IsOverBudget => SpentAmount > AllocatedAmount;
}