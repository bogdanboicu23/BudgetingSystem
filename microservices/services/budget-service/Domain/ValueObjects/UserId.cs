namespace BudgetService.Domain.ValueObjects;

public record UserId
{
    public Guid Value { get; init; }

    private UserId(Guid value)
    {
        Value = value;
    }

    public static UserId From(Guid value) => new(value);

    public static UserId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException("Invalid UserId format", nameof(value));

        return new(guid);
    }

    public override string ToString() => Value.ToString();

    public static implicit operator Guid(UserId userId) => userId.Value;
}