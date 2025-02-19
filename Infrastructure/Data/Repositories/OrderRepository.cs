using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class OrderRepository(ShopDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
}