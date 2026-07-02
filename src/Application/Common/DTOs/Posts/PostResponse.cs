using Application.Common.DTOs.Comments;
using Application.Common.DTOs.Tags;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.DTOs.Posts;

public record PostResponse(Guid Id, string Title, string Excerpt, string AuthorName,
    int LikeCount, int CommentCount, DateTime CreatedAt, PostStatus Status, 
    List<TagResponse>? TagResponses = null,List<CommentResponse>? CommentResponses = null );
