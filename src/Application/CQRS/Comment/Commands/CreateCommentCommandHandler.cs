using Application.Common;
using Application.Common.DTOs.Comments;
using Application.Interfaces;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.CQRS.Comment.Commands;
public record CreateCommentCommand(Guid PostId, Guid UserId, string Text) : IRequest<Result<CommentResponse>>;


public class CreateCommentCommandHandler(
    ICommentRepository _commentRepo,
    IPostRepository _postRepo,
    IUserRepository _userRepo,
    IUnitOfWork _unitOfWork) : IRequestHandler<CreateCommentCommand, Result<CommentResponse>>
{
    public async Task<Result<CommentResponse>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepo.GetByIdAsync(request.PostId);
        if (post is null)
            return Result<CommentResponse>.Fail("Post not found");

        var comment = new Domain.Entities.Comment(request.PostId, request.UserId, request.Text);

        await _commentRepo.AddAsync(comment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var user = await _userRepo.GetByIdAsync(request.UserId);

        var response = new CommentResponse(
            comment.Id,
            user?.FullName ?? "Unknown",
            comment.CommentText,
            comment.CreatedDateTime
        );

        return Result<CommentResponse>.Success(response, "Comment added successfully");
    }
}

public class CreateCommentCommandValidator : AbstractValidator<CreateCommentCommand>
{
    public CreateCommentCommandValidator()
    {
        RuleFor(x => x.PostId)
            .NotEmpty().WithMessage("Post ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.Text)
            .NotEmpty().WithMessage("Comment text is required")
            .MinimumLength(1).WithMessage("Comment must be at least 1 character")
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters");
    }
}