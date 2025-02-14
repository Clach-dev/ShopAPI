namespace Domain.Entities;

public class RefreshToken
{
    public Guid Token { get; set; }
    
    public DateTime ExpirationDate { get; set; }
    
    public virtual User? User { get; set; }
}