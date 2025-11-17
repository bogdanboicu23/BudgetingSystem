namespace BudgetService.Domain.ValueObjects;

public record BudgetId
{
    public Guid Value { get; init; }

    private BudgetId(Guid value)
    {
        Value = value;
    }

    public static BudgetId New() => new(Guid.NewGuid());

    public static BudgetId From(Guid value) => new(value);

    public static BudgetId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("Invalid BudgetId format", nameof(value));

        return new(guid);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(BudgetId budgetId) => budgetId.Value;
}