using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface ICommentRepository : IGenericRepository<Comment>
{
    Task<IReadOnlyList<Comment>> GetCommentsByPostIdAsync(Guid postId);
    Task<IReadOnlyList<Comment>> GetCommentsByUserIdAsync(Guid userId);
    Task<int> GetCommentCountByPostIdAsync(Guid postId);
}
