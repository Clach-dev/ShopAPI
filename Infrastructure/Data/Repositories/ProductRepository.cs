using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class ProductRepository (ShopDbContext context) : BaseRepository<Product>(context), IProductRepository
{
}