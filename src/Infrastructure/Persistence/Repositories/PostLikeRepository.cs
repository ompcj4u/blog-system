using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;
public class PostLikeRepository : GenericRepository<PostLike>, IPostLikeRepository
{
    public PostLikeRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> GetLikeCountByPostIdAsync(Guid postId)
    {
        return await dbSet.CountAsync(t => t.PostId == postId);
    }

    public async Task<bool> IsLikedByUserAsync(Guid postId, Guid userId)
    {
        return await dbSet.Where(t => t.UserId == userId && t.PostId == postId).AnyAsync();
    }

    public async Task<bool> ToggleLikeAsync(Guid postId, Guid userId)
    {

        var existingLike = await dbSet.FirstOrDefaultAsync(t => t.PostId == postId && t.UserId == userId);

        if (existingLike != null)
        {
            // remove Like
            dbSet.Remove(existingLike);
            return false;
        }
        else
        {
            var entity = new PostLike(postId, userId);
            await dbSet.AddAsync(entity);
            return true;
            // Add Like
        }
    }
}
