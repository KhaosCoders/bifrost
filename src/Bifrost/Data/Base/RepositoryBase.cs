using Bifrost.Utils.Guards;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Data.Base;

/// <summary>
/// Base class for all repositories.
/// </summary>
/// <typeparam name="TEntity">Type of the repository</typeparam>
/// <param name="dbContext">EntityFramework DbContext</param>
public abstract class RepositoryBase<TEntity>(DbContext dbContext) : IRepositoryBase<TEntity> where TEntity : class, IEntity
{
    private readonly DbContext dbContext = dbContext;

    /// <summary>
    /// Returns all entities of the repository.
    /// </summary>
    /// <remarks>
    /// No EntityFramework tracking is used.
    /// </remarks>
    /// <returns>Queryable list of entities</returns>
    public IQueryable<TEntity> QueryAll() =>
        dbContext.Set<TEntity>().AsNoTracking();

    /// <summary>
    /// Returns a single entity by its id.
    /// </summary>
    /// <remarks>
    /// No EntityFramework tracking is used.
    /// </remarks>
    /// <param name="id">Id of entity</param>
    /// <returns>Found entity or null</returns>
    public Task<TEntity?> GetByIdAsync(string id)
    {
        Guard.Against.StringIsNullOrWhitespace(id);
        return QueryAll().FirstOrDefaultAsync(e => e.Id == id);
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">New entity</param>
    /// <param name="isBulkOperation">Indicates if this is a bulk operation, so no SaveChanges is called</param>
    /// <returns>
    /// Awaitable task.
    /// </returns>
    public async Task CreateAsync(TEntity entity, bool isBulkOperation = false)
    {
        await dbContext.Set<TEntity>().AddAsync(entity);
        if (isBulkOperation) return;
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="entity">Entity to update</param>
    /// <param name="isBulkOperation">Indicates if this is a bulk operation, so no SaveChanges is called</param>
    /// <returns>
    /// Awaitable task.
    /// </returns>
    public async Task UpdateAsync(TEntity entity, bool isBulkOperation = false)
    {
        dbContext.Set<TEntity>().Update(entity);
        if (isBulkOperation) return;
        await dbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes an entity by its id.
    /// </summary>
    /// <param name="id">Id of entity to delete</param>
    /// <param name="isBulkOperation">Indicates if this is a bulk operation, so no SaveChanges is called</param>
    /// <returns>
    /// Awaitable task.
    /// </returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public async Task DeleteAsync(string id, bool isBulkOperation = false)
    {
        Guard.Against.StringIsNullOrWhitespace(id);
        var entity = await GetByIdAsync(id) ?? throw new EntityNotFoundException();
        dbContext.Set<TEntity>().Remove(entity);
        if (isBulkOperation) return;
        await dbContext.SaveChangesAsync();
    }
}
