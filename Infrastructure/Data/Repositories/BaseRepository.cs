using System.Linq.Expressions;
using Domain.Entities;
using Domain.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public abstract class BaseRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _entities;
    
    protected BaseRepository(ShopDbContext context)
    {
        _entities = context.Set<TEntity>();
    }
    
    public async Task<(IEnumerable<TEntity>, int)> GetAllAsync(
        PageInfo pageInfo,
        CancellationToken cancellationToken = default)
    {
        var entities = await _entities
            .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
            .Take(pageInfo.PageSize)
            .ToListAsync(cancellationToken);
        
        var totalCount = await _entities.CountAsync(cancellationToken);

        return (entities, totalCount);
    }

    public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _entities.FindAsync(id, cancellationToken);
    }

    public async Task<IEnumerable<TEntity>> GetByPredicateAsync(
        Expression<Func<TEntity, bool>> predicate,
        PageInfo pageInfo,
        CancellationToken cancellationToken = default)
    {
        return await _entities
            .Where(predicate)
            .Skip((pageInfo.PageNumber - 1) * pageInfo.PageSize)
            .Take(pageInfo.PageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _entities.AddAsync(entity, cancellationToken);
    }

    public Task Delete(TEntity entity)
    {
        return Task.FromResult(_entities.Remove(entity));
    }
}