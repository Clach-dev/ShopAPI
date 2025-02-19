using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class OrderItemRepository(ShopDbContext context) : BaseRepository<OrderItem>(context), IOrderItemRepository
{
}