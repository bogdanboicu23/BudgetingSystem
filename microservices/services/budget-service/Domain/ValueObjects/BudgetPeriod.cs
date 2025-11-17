namespace BudgetService.Domain.ValueObjects;

public record BudgetPeriod
{
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public BudgetPeriodType PeriodType { get; init; }

    private BudgetPeriod(DateTime startDate, DateTime endDate, BudgetPeriodType periodType)
    {
        if (startDate >= endDate)
            throw new ArgumentException("Start date must be before end date");

        StartDate = startDate.Date;
        EndDate = endDate.Date;
        PeriodType = periodType;
    }

    public static BudgetPeriod Create(DateTime startDate, DateTime endDate, BudgetPeriodType periodType)
        => new(startDate, endDate, periodType);

    public static BudgetPeriod Monthly(DateTime startDate)
        => new(startDate.Date, startDate.AddMonths(1).Date, BudgetPeriodType.Monthly);

    public static BudgetPeriod Yearly(DateTime startDate)
        => new(startDate.Date, startDate.AddYears(1).Date, BudgetPeriodType.Yearly);

    public static BudgetPeriod Weekly(DateTime startDate)
        => new(startDate.Date, startDate.AddDays(7).Date, BudgetPeriodType.Weekly);

    public bool IsActive => DateTime.UtcNow.Date >= StartDate && DateTime.UtcNow.Date <= EndDate;

    public bool IsExpired => DateTime.UtcNow.Date > EndDate;

    public int DaysRemaining => IsExpired ? 0 : (EndDate - DateTime.UtcNow.Date).Days;

    public TimeSpan Duration => EndDate - StartDate;
}

public enum BudgetPeriodType
{
    Weekly,
    Monthly,
    Yearly,
    Custom
}