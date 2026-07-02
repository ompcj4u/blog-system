using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Comment : BaseEntity
{
    private Comment() { }
    public Comment(Guid postId, Guid userId, string text)
    {
        PostId = postId;
        UserId = userId;
        CommentText = text;
    }
    public Guid PostId { get; set; }
    public Guid UserId { get; set; }
    public string CommentText { get; set; }

    public Post Post { get; set; }
    public User User { get; set; }
}
