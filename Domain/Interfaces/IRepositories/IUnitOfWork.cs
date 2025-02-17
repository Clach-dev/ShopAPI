using Domain.Interfaces.IRepositories;

namespace Domain.Interfaces.IRepositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    
    IProductRepository Products { get; }
    
    IOrderRepository Orders { get; }
    
    IOrderItemRepository OrderItems { get; }
    
    IUserRepository Users { get; }
    
    IRefreshTokenRepository RefreshTokens { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}