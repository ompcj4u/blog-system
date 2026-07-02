using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories;
public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(AppDbContext context) : base(context)
    {
    }

    public async Task AddPostTagAsync(PostTag postTag)
    {
        await context.PostTags.AddAsync(postTag);
    }

    public async Task<Post?> GetPostDetailByIdAsync(Guid postId)
    {
        return await dbSet
           .Include(p => p.Author)
            .Include(p => p.LikedBy)
                .ThenInclude(pl => pl.User)
            .Include(p => p.Comments)
                .ThenInclude(c => c.User)
            .Include(p => p.PostTags)
                .ThenInclude(pt => pt.Tag)
            .FirstOrDefaultAsync(p => p.Id == postId);
    }

    public async Task<IReadOnlyList<Post>> GetPostsByAuthorIdAsync(Guid authorId)
    {
        return await dbSet.Where(t => t.AuthorId == authorId)
            .Include(p => p.LikedBy)
            .Include(p=>p.Comments)
            .Include(t => t.Author).
            ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> GetPostsByStatusAsync(Guid userId, PostStatus status)
    {
        return await dbSet
            .Where(p => p.AuthorId == userId && p.Status == status)
            .OrderByDescending(p => p.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> GetPostsByTagIdAsync(Guid tagId)
    {
        return await dbSet
            .Where(p => p.PostTags.Any(pt => pt.TagId == tagId) && p.Status == PostStatus.Published)
            .Include(p => p.Author)
            .Include(p => p.LikedBy)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedDateTime)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Post>> GetPublishedPostsAsync()
    {
        return await dbSet.Where(t => t.Status == PostStatus.Published && t.IsDeleted == false)
            .Include(p => p.Author)
            .OrderByDescending(t => t.CreatedDateTime)
            .ToListAsync();
    }

    public async Task RemovePostTagAsync(Guid postId, Guid tagId)
    {
        var postTag = await context.PostTags
            .FirstOrDefaultAsync(pt => pt.PostId == postId && pt.TagId == tagId);

        if (postTag != null)
            context.PostTags.Remove(postTag);
    }

}
