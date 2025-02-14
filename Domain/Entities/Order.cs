namespace Domain.Entities;

public class Order : BaseEntity
{
    public Guid OrderItemId { get; set; }
    
    public Guid UserId { get; set; }
    
    public double TotalPrice { get; set; } 
    
    public string Status { get; set; } = String.Empty;
    
    public DateTime DeliveryDate { get; set; }
    
    public virtual User? User { get; set; } 
    
    public virtual IEnumerable<OrderItem>? OrderItems { get; set; }
}