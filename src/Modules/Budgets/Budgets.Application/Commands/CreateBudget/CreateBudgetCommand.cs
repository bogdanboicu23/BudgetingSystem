using MediatR;

namespace Budgets.Application.Commands.CreateBudget;

public class CreateBudgetCommand : IRequest<Guid>
{
    public string Name { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }

    public CreateBudgetCommand(string name, DateTime periodStart, DateTime periodEnd)
    {
        Name = name;
        PeriodStart = periodStart;
        PeriodEnd = periodEnd;
    }
}