using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class CategoryRepository(ShopDbContext context) : BaseRepository<Category>(context), ICategoryRepository
{
}