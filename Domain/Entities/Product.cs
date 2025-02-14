namespace Domain.Entities;

public class Product : BaseEntitie
{
    public string Name { get; set; } = String.Empty;
    
    public string Description { get; set; } = String.Empty;
    
    public double Price { get; set; }
    
    public int Amount { get; set; }
    
    public IEnumerable<Category>? Categories { get; set; }
}