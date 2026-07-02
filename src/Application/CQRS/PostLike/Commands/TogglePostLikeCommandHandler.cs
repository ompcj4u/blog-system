using Application.Common;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.PostLike.Commands;
public record TogglePostLikeCommand(Guid PostId, Guid UserId) : IRequest<Result<bool>>;
public class TogglePostLikeCommandHandler(IPostLikeRepository _postLikeRepo, IPostRepository _postRepo) : IRequestHandler<TogglePostLikeCommand, Result<bool>>
{
    

    public async Task<Result<bool>> Handle(TogglePostLikeCommand request, CancellationToken cancellationToken)
    {
        // Check post exists
        var post = await _postRepo.GetByIdAsync(request.PostId);
        if (post == null)
            return Result<bool>.Fail("Post not found");

        var isLiked = await _postLikeRepo.ToggleLikeAsync(request.PostId, request.UserId);

        return Result<bool>.Success(isLiked,
            isLiked ? "Post liked" : "Post unliked");
    }

}
