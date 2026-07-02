using Application.Common;
using Application.Common.DTOs.Posts;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Commands;

public record UpdatePostCommand(string title, string content, PostStatus Status, List<Guid> tags,Guid postId) : IRequest<Result<PostResponse>>;

public class UpdatePostCommandHandler(IPostRepository _postRepo, IUnitOfWork _unitOfWork)
    : IRequestHandler<UpdatePostCommand, Result<PostResponse>>
{
    public async Task<Result<PostResponse>> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            var post = await _postRepo.GetPostDetailByIdAsync(request.postId);
            if (post is null)
                return Result<PostResponse>.Fail("Post not found");

            post.Update(request.title, new Content(request.content), request.Status);

            // حذف تگ‌های قدیمی
            foreach (var existingTag in post.PostTags.ToList())
            {
                await _postRepo.RemovePostTagAsync(post.Id, existingTag.TagId);
            }

            // اضافه کردن تگ‌های جدید
            foreach (var tagId in request.tags)
            {
                await _postRepo.AddPostTagAsync(new PostTag(post.Id, tagId));
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var response = new PostResponse(
                post.Id, post.PostTitle, post.PostContent.Excerpt,
                post.Author?.FullName ?? "Unknown",
                post.LikedBy?.Count ?? 0,
                post.Comments?.Count ?? 0,
                post.CreatedDateTime, post.Status);

            return Result<PostResponse>.Success(response, "Post updated successfully");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<PostResponse>.Fail(ex.Message);
        }
    }
}


