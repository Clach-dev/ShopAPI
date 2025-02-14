using System.Linq.Expressions;

namespace Domain.Interfaces.IRepositories;

public interface IRepository<TEntity> where TEntity : class
{
    Task<(IEnumerable<TEntity>, int)> GetAllAsync(CancellationToken cancellationToken = default);
    
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<TEntity>> GetByPredicateAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    Task CreateAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    Task Delete(TEntity entity);
}