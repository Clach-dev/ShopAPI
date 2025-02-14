namespace Domain.Entities;

public class Order : BaseEntitie
{
    public Guid OrderItemId { get; set; }
    
    public Guid UserId { get; set; }
    
    public double TotalPrice { get; set; } 
    
    public string Status { get; set; } = String.Empty;
    
    public DateTime DeliveryDate { get; set; }
    
    public virtual User? User { get; set; } 
    
    public virtual OrderItem? OrderItem { get; set; }
}