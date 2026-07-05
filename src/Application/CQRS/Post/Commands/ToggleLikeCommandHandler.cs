using Application.Common;
using Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Commands;
public record ToggleLikeCommand(Guid PostId, Guid UserId) : IRequest<Result<bool>>;

public class ToggleLikeCommandHandler(IPostLikeRepository _postLikeRepo, IPostRepository _postRepo, IUnitOfWork _unitOfWork)
    : IRequestHandler<ToggleLikeCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepo.GetByIdAsync(request.PostId);
        if (post is null)
            return Result<bool>.Fail("Post not found");

        var isLiked = await _postLikeRepo.ToggleLikeAsync(request.PostId, request.UserId);
        await _unitOfWork.SaveChangesAsync();
        return Result<bool>.Success(isLiked, isLiked ? "Post liked" : "Post unliked");
    }
}