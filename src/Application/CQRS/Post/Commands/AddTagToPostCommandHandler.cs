using Application.Common;
using Application.Interfaces;
using Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Commands;
public record AddTagToPostCommand(Guid PostId, Guid TagId) : IRequest<Result>;

public class AddTagToPostCommandHandler(IPostRepository _postRepo, ITagRepository _tagRepo, IUnitOfWork _unitOfWork)
    : IRequestHandler<AddTagToPostCommand, Result>
{
    public async Task<Result> Handle(AddTagToPostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepo.GetByIdAsync(request.PostId);
        if (post is null)
            return Result.Fail("Post not found");

        var tag = await _tagRepo.GetByIdAsync(request.TagId);
        if (tag is null)
            return Result.Fail("Tag not found");

        var exists = await _postRepo.ExistAsync(p => p.PostTags.Any(pt => pt.PostId == request.PostId && pt.TagId == request.TagId));
        if (exists)
            return Result.Fail("Tag already added to this post");

        await _postRepo.AddPostTagAsync(new PostTag(request.PostId, request.TagId));
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Tag added to post");
    }
}