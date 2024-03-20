using Bifrost.Models;

namespace Bifrost.Data.Base;

public interface IRepositoryBase<TEntity> where TEntity : class, IEntity
{
    Task CreateAsync(TEntity entity, bool isBulkOperation = false);
    Task DeleteAsync(string id, bool isBulkOperation = false);
    Task<TEntity?> GetByIdAsync(string id);
    IQueryable<TEntity> QueryAll();
    Task UpdateAsync(TEntity entity, bool isBulkOperation = false);
}