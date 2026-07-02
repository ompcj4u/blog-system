using Application.Common;
using Application.Common.DTOs.Posts;
using Application.Common.Models;
using Application.Interfaces;
using Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Queries;
public record GetPublishedPostsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PaginatedResult<PostResponse>>;

public class GetPublishedPostsQueryHandler(IPostRepository _postRepo)
    : IRequestHandler<GetPublishedPostsQuery, PaginatedResult<PostResponse>>
{
    public async Task<PaginatedResult<PostResponse>> Handle(GetPublishedPostsQuery request, CancellationToken cancellationToken)
    {
        var query = _postRepo.GetIQueryable()
            .Where(p => p.Status == PostStatus.Published)
            .OrderByDescending(p => p.CreatedDateTime)
            .Select(p => new PostResponse(
                p.Id, p.PostTitle, p.PostContent.Excerpt,
                p.Author!.FullName, p.LikedBy.Count, p.Comments.Count,
                p.CreatedDateTime, p.Status,null,null));

        var paginatedList = await PaginatedList<PostResponse>.CreateAsync(query, request.PageNumber, request.PageSize);
        return PaginatedResult<PostResponse>.Success(paginatedList);
    }
}
