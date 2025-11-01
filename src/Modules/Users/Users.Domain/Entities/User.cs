using Budgets.Domain.Entities;
using Monolith_BudgetSystem.Shared.Entities;

namespace Users.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    private readonly List<Budget> _budgets = new();
    public IReadOnlyCollection<Budget> Budgets => _budgets.AsReadOnly();
    
}