using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IPostRepository : IGenericRepository<Post>
{
    Task<IReadOnlyList<Post>> GetPublishedPostsAsync();
    Task<IReadOnlyList<Post>> GetPostsByAuthorIdAsync(Guid authorId);
    Task<IReadOnlyList<Post>> GetPostsByStatusAsync(Guid userId, PostStatus status);
    Task<Post?> GetPostDetailByIdAsync(Guid postId);
    Task<IReadOnlyList<Post>> GetPostsByTagIdAsync(Guid tagId);
    Task AddPostTagAsync(PostTag postTag);
    Task RemovePostTagAsync(Guid postId, Guid tagId);

}
