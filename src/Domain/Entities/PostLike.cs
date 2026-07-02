using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class PostLike : BaseEntity
{
    private PostLike() { }
    public PostLike(Guid postId, Guid userId)
    {
        PostId = postId;
        UserId = userId;
    }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }

    public User User { get; set; }
    public Post Post { get; set; }

}
