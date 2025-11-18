namespace ExpenseService.Domain.ValueObjects;

public record ExpenseId(Guid Value)
{
    public static ExpenseId New() => new(Guid.NewGuid());
    public static ExpenseId From(Guid value) => new(value);

    public static implicit operator Guid(ExpenseId expenseId) => expenseId.Value;
    public static implicit operator ExpenseId(Guid value) => new(value);

    public override string ToString() => Value.ToString();
}