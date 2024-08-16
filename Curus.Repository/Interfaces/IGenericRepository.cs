using Curus.Repository.Entities;

namespace Curus.Repository.Interfaces;

public interface IGenericRepository<TEntity> where TEntity : BaseEntity<int>
{
    Task<List<TEntity>> GetAllAsync();
    
    Task<TEntity?> GetByIdAsync(int id);
    
    Task<TEntity> AddAsync(TEntity entity);
    
    Task AddRange(List<TEntity> entities);
    
    Task Update(TEntity entity);
    
    Task UpdateRange(List<TEntity> entities);
    
    Task SoftRemove(TEntity entity);
    
    Task SoftRemoveRange(List<TEntity> entities);
}