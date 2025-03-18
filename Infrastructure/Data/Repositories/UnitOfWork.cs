using Azure.Storage.Blobs;
    using Domain.Interfaces.IRepositories;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data.Repositories;

public class UnitOfWork(BlobServiceClient blobServiceClient, ShopDbContext context, IConfiguration configuration) : IUnitOfWork
{
    private ICategoryRepository? _categoryRepository;

    private IProductRepository? _productRepository;

    private IProductImageRepository? _productImageRepository;
        
    private IOrderRepository? _orderRepository;

    private IOrderItemRepository? _orderItemRepository;

    private IUserRepository? _userRepository;

    private IRefreshTokenRepository? _refreshTokenRepository;
    
    public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(context);
    
    public IProductRepository Products => _productRepository ??= new ProductRepository(context);
    
    public IProductImageRepository ProductImages => _productImageRepository ??= new ProductImageRepository(blobServiceClient, configuration);
    
    public IOrderRepository Orders => _orderRepository ??= new OrderRepository(context);
    
    public IOrderItemRepository OrderItems => _orderItemRepository ??= new OrderItemRepository(context);
    
    public IUserRepository Users => _userRepository ??= new UserRepository(context);
    
    public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(context);
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    
    // public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    // {
    //     return await context.Database.BeginTransactionAsync(cancellationToken);
    // }
    
    
    
    public void Dispose()
    {
        context.Dispose();
    }
}