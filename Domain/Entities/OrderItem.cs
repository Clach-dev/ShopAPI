namespace Domain.Entities;

public class OrderItem : BaseEntitie
{
    public Guid ProductId { get; set; }

    public Guid OrderId { get; set; }

    public int Amount { get; set; }
    
    public virtual Product? Product { get; set; }
    
    public virtual Order? Order { get; set; }
}
