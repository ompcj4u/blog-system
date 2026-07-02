using Application.Common;
using Application.Common.DTOs.Comments;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Comment.Queries;
public record GetCommentsByPostQuery(Guid PostId) : IRequest<Result<List<CommentResponse>>>;

public class GetCommentsByPostQueryHandler(ICommentRepository _commentRepo)
    : IRequestHandler<GetCommentsByPostQuery, Result<List<CommentResponse>>>
{
    public async Task<Result<List<CommentResponse>>> Handle(GetCommentsByPostQuery request, CancellationToken cancellationToken)
    {
        var comments = await _commentRepo.GetCommentsByPostIdAsync(request.PostId);

        var response = comments.Select(c => new CommentResponse(
            c.Id,
            c.User?.FullName ?? "Unknown",
            c.CommentText,
            c.CreatedDateTime
        )).ToList();

        return Result<List<CommentResponse>>.Success(response,"comments of a post");
    }
}
