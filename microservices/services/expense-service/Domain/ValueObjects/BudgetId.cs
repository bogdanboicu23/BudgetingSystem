namespace ExpenseService.Domain.ValueObjects;

public record BudgetId(Guid Value)
{
    public static BudgetId New() => new(Guid.NewGuid());
    public static BudgetId From(Guid value) => new(value);

    public static implicit operator Guid(BudgetId budgetId) => budgetId.Value;
    public static implicit operator BudgetId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}