using Application.Common;
using Application.Common.DTOs.Posts;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Queries;
public record GetPostsByTagQuery(Guid TagId) : IRequest<Result<List<PostResponse>>>;

public class GetPostsByTagQueryHandler(IPostRepository _postRepo)
    : IRequestHandler<GetPostsByTagQuery, Result<List<PostResponse>>>
{
    public async Task<Result<List<PostResponse>>> Handle(GetPostsByTagQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepo.GetPostsByTagIdAsync(request.TagId);

        var response = posts.Select(p => new PostResponse(
            p.Id, p.PostTitle, p.PostContent.Excerpt,
            p.Author?.FullName ?? "Unknown",
            p.LikedBy?.Count ?? 0,
            p.Comments?.Count ?? 0,
            p.CreatedDateTime, p.Status,
            null,null
        )).ToList();

        return Result<List<PostResponse>>.Success(response,"list of posts");
    }
}
