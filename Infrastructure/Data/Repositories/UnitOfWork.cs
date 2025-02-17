    using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class UnitOfWork(ShopDbContext context) : IUnitOfWork
{
    private ICategoryRepository? _categoryRepository;

    private IProductRepository? _productRepository;

    private IOrderRepository? _orderRepository;

    private IOrderItemRepository? _orderItemRepository;

    private IUserRepository? _userRepository;

    private IRefreshTokenRepository? _refreshTokenRepository;
    
    public ICategoryRepository Categories => _categoryRepository ??= new CategoryRepository(context);
    
    public IProductRepository Products => _productRepository ??= new ProductRepository(context);
    
    public IOrderRepository Orders => _orderRepository ??= new OrderRepository(context);
    
    public IOrderItemRepository OrderItems => _orderItemRepository ??= new OrderItemRepository(context);
    
    public IUserRepository Users => _userRepository ??= new UserRepository(context);
    
    public IRefreshTokenRepository RefreshTokens => _refreshTokenRepository ??= new RefreshTokenRepository(context);
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }
    
    public void Dispose()
    {
        context.Dispose();
    }
}