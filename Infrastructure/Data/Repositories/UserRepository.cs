using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class UserRepository(ShopDbContext context) : BaseRepository<User>(context), IUserRepository
{
}