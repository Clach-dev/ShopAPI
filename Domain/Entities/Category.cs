namespace Domain.Entities;

public class Category : BaseEntitie
{
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    public IEnumerable<Product>? Product { get; set; }
}