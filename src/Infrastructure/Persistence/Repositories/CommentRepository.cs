using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;
public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(AppDbContext context) : base(context) { }

    public async Task<IReadOnlyList<Comment>> GetCommentsByPostIdAsync(Guid postId)
    {
        return await dbSet
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Comment>> GetCommentsByUserIdAsync(Guid userId)
    {
        return await dbSet
            .Where(c => c.UserId == userId)
            .Include(c => c.Post)
            .OrderByDescending(c => c.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<int> GetCommentCountByPostIdAsync(Guid postId)
    {
        return await dbSet.CountAsync(c => c.PostId == postId);
    }
}
