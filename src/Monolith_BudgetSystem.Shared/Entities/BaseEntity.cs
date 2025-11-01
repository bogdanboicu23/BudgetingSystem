namespace Monolith_BudgetSystem.Shared.Entities;

public class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string? ModifiedBy { get; set; }
    
    // Proprietăti pentru fiecare item din cheltuieli/venituri/buget
    public string Name { get; set; }
    public decimal Amount { get; set; }
}