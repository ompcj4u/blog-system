using Domain.Common;
using Domain.Enums;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities;
public class Post : BaseEntity
{
    private Post() { }

    public Post(Guid creator, string title)
    {
        AuthorId = creator;
        PostTitle = title;
    }
    public Guid AuthorId { get; set; }
    public string PostTitle { get; set; }
    public Content PostContent { get; set; }
    public PostStatus Status { get; set; }
    public bool IsPublished { get; set; }

    public User Author { get; set; }
    public IReadOnlyCollection<PostTag> PostTags { get; set; }
    public IReadOnlyCollection<PostLike> LikedBy { get; set; }
    public IReadOnlyCollection<Comment> Comments { get; set; }

    public static Post Create(Guid userId, string title, Content content) => new Post(userId, title) { PostContent = content, Status = PostStatus.Draft };

    public void Update(string title, Content content, PostStatus status)
    {
        PostTitle = title;
        PostContent = content;
        Status = status;
        ModifiedDateTime = DateTime.UtcNow;
    }
}
