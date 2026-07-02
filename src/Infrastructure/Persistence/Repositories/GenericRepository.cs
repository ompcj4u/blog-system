using Domain.Common;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Repositories;
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext context;
    protected readonly DbSet<T> dbSet;
    public GenericRepository(AppDbContext context)
    {
        this.context = context;
        dbSet = context.Set<T>();
    }
    public async Task<Guid> AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
        return entity.Id;

    }

    public async Task<bool> DeleteOneAsync(Guid Id)
    {
        var entity = await GetByIdAsync(Id);
        if(entity == null) return false;
        entity.IsDeleted = true;
        entity.DeletedDateTime = DateTime.UtcNow;
        dbSet.Update(entity);
        return true;
    }

    public async Task<bool> ExistAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.AnyAsync(predicate);
    }

    public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
    {
        return await dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await dbSet.FirstOrDefaultAsync(t => t.Id == id);
    }

    public IQueryable<T> GetIQueryable()
    {
        return dbSet.AsQueryable();
    }

    public Task<bool> UpdateAsync(T entity)
    {
        dbSet.Update(entity);
        return Task.FromResult(true);
    }
}
