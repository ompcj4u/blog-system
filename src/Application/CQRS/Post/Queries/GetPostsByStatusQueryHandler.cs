using Application.Common;
using Application.Common.DTOs.Posts;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Queries;
public record GetPostsByStatusQuery(Guid UserId, PostStatus Status) : IRequest<Result<List<PostResponse>>>;

public class GetPostsByStatusQueryHandler(IPostRepository _postRepo)
    : IRequestHandler<GetPostsByStatusQuery, Result<List<PostResponse>>>
{
    public async Task<Result<List<PostResponse>>> Handle(GetPostsByStatusQuery request, CancellationToken cancellationToken)
    {
        var posts = await _postRepo.GetPostsByStatusAsync(request.UserId, request.Status);

        var response = posts.Select(p => new PostResponse(
            p.Id, p.PostTitle, p.PostContent.Excerpt,
            p.Author?.FullName ?? "Unknown",
            p.LikedBy?.Count ?? 0,
            p.Comments?.Count ?? 0,
            p.CreatedDateTime, p.Status
        )).ToList();

        return Result<List<PostResponse>>.Success(response, "list of posts");
    }
}