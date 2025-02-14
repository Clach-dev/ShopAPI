namespace Domain.Entities;

public class Product : BaseEntity
{
    public string Name { get; set; } = String.Empty;
    
    public string Description { get; set; } = String.Empty;
    
    public double Price { get; set; }
    
    public int Amount { get; set; }
    
    public virtual IEnumerable<Category>? Categories { get; set; }
    
    public virtual IEnumerable<OrderItem>? OrderItems { get; set; }
}