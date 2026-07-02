using Domain.Entities;
using Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces;
public interface IPostLikeRepository : IGenericRepository<PostLike>
{
    Task<bool> IsLikedByUserAsync(Guid postId, Guid userId);
    Task<int> GetLikeCountByPostIdAsync(Guid postId);
    Task<bool> ToggleLikeAsync(Guid postId, Guid userId);
}
