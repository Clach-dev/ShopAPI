using Domain.Entities;
using Domain.Interfaces.IRepositories;

namespace Infrastructure.Data.Repositories;

public class RefreshTokenRepository(ShopDbContext context) : BaseRepository<RefreshToken>(context), IRefreshTokenRepository
{
}