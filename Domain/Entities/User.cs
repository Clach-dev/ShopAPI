using Domain.Enums;

namespace Domain.Entities;

public class User : BaseEntitie
{
    public Guid RefreshTokenId { get; set; }
    
    public string PhoneNumber { get; set; } = String.Empty;
    
    public string Password { get; set; } = String.Empty;
    
    public string LastName { get; set; } = String.Empty;
    
    public string FirstName { get; set; } = String.Empty;
    
    public string? MiddleName { get; set; }
    
    public DateTime? BirthDate { get; set; }
    
    public string? Email { get; set; }
    
    public Roles Role { get; set; } 
    
    public virtual RefreshToken? RefreshToken { get; set; }
}