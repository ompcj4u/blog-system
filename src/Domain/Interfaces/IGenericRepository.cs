using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T,bool>> predicate);
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T,bool>> predicate);
    Task<Guid> AddAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteOneAsync(Guid Id);
    Task<bool> ExistAsync(Expression<Func<T, bool>> predicate);
    IQueryable<T> GetIQueryable();
}
