using Budgets.Domain.Entities;
using MediatR;

namespace Budgets.Application.Commands.AddPlannedItem;

public class AddPlannedItemCommand : IRequest<AddPlannedItemResponse>
{
    public Guid BudgetId { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal PlannedAmount { get; set; }
    public string Category { get; set; } = "";
    public PlannedItemType Type { get; set; }
    public DateTime? DueDate { get; set; }
    public bool IsRecurring { get; set; }
    public RecurrenceFrequency? Frequency { get; set; }
}

public class AddPlannedItemResponse
{
    public Guid PlannedItemId { get; set; }
    public string Name { get; set; } = "";
    public decimal PlannedAmount { get; set; }
    public PlannedItemType Type { get; set; }
    public string Category { get; set; } = "";
    public bool IsRecurring { get; set; }
    public DateTime? DueDate { get; set; }
}