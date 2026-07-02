using Application.Common;
using Application.Common.DTOs.Posts;
using Application.CQRS.Post.Queries;
using Application.Interfaces;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Post.Commands;
public class CreatePostCommandHandler(IPostRepository repoPost, IUserRepository repoUser, IUnitOfWork unitOfWork) :
    IRequestHandler<CreatePostCommand, Result<PostResponse>>
{
    public async Task<Result<PostResponse>> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync();
            var post = Domain.Entities.Post.Create(request.userId, request.title , new Domain.ValueObjects.Content(request.content));
            await repoPost.AddAsync(post);
            foreach (var tagId in request.tags)
            {
                var postTag = new PostTag(post.Id, tagId);
                await repoPost.AddPostTagAsync(postTag);
            }

            await unitOfWork.SaveChangesAsync();
            await unitOfWork.CommitTransactionAsync();
            var user = await repoUser.GetByIdAsync(request.userId);
            var postResponse = new PostResponse(post.Id, post.PostTitle, post.PostContent.Excerpt, user.FullName, 0, 0, post.CreatedDateTime, Domain.Enums.PostStatus.Draft);
            return Result<PostResponse>.Success(postResponse, "Successfully inserted");

        }
        catch (Exception ex)
        {
            await unitOfWork.RollbackTransactionAsync();
            return Result<PostResponse>.Fail(ex.Message);
        }
    }
}

public record CreatePostCommand(string title, string content, List<Guid> tags, Guid userId) : IRequest<Result<PostResponse>>;

public class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostCommandValidator()
    {
        RuleFor(x => x.title).NotEmpty().WithMessage("عنوان الزامی است")
            .MinimumLength(5).WithMessage("عنوان حداقل سه کاراکتر")
            .MaximumLength(50).WithMessage("عنوان حداکثر 50 کاراکتر");

        RuleFor(x => x.content).NotEmpty().WithMessage("بدنه پست الزامی است")
            .MinimumLength(10).WithMessage("بدنه پست حداقل 10 کاراکتر")
            .MaximumLength(5000).WithMessage("بدنه پست حداکثر 5000 کاراکتر");

        RuleFor(t => t.userId).NotEmpty().WithMessage("کاربر ایجاد کننده پست مشخص نیست");
    }
}