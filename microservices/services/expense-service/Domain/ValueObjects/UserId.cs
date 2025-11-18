namespace ExpenseService.Domain.ValueObjects;

public record UserId(Guid Value)
{
    public static UserId New() => new(Guid.NewGuid());
    public static UserId From(Guid value) => new(value);

    public static implicit operator Guid(UserId userId) => userId.Value;
    public static implicit operator UserId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}