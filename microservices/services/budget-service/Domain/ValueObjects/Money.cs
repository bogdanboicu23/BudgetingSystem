namespace BudgetService.Domain.ValueObjects;

public record Money
{
    public decimal Amount { get; init; }
    public string Currency { get; init; }

    private Money(decimal amount, string currency)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        Amount = Math.Round(amount, 2);
        Currency = currency.ToUpperInvariant();
    }

    public static Money Create(decimal amount, string currency) => new(amount, currency);

    public static Money Zero(string currency) => new(0, currency);

    public static Money Add(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    public static Money Subtract(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        var result = left.Amount - right.Amount;
        if (result < 0)
            throw new InvalidOperationException("Result cannot be negative");

        return new Money(result, left.Currency);
    }

    public static Money Multiply(Money money, decimal factor)
    {
        if (factor < 0)
            throw new ArgumentException("Factor cannot be negative", nameof(factor));

        return new Money(money.Amount * factor, money.Currency);
    }

    public override string ToString() => $"{Amount:C} {Currency}";
}