using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Interfaces.IRepositories;

public interface IUnitOfWork : IDisposable
{
    ICategoryRepository Categories { get; }
    
    IProductRepository Products { get; }
    
    IProductImageRepository ProductImages { get; }
    
    IOrderRepository Orders { get; }
    
    IOrderItemRepository OrderItems { get; }
    
    IUserRepository Users { get; }
    
    IRefreshTokenRepository RefreshTokens { get; }
    
    // Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);

    
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}