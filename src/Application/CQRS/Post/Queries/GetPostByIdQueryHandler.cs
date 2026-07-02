using Application.Common;
using Application.Common.DTOs.Comments;
using Application.Common.DTOs.Posts;
using Application.Common.DTOs.Tags;
using Application.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.CQRS.Post.Queries;
public class GetPostByIdQueryHandler(IPostRepository _postRepo)
    : IRequestHandler<GetPostByIdQuery, Result<PostResponse>>
{
    public async Task<Result<PostResponse>> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepo.GetPostDetailByIdAsync(request.postId);
        if (post is null)
            return Result<PostResponse>.Fail("Post not found");

        var response = new PostResponse(
            post.Id, post.PostTitle, post.PostContent.Excerpt,
            post.Author?.FullName ?? "Unknown",
            post.LikedBy?.Count ?? 0,
            post.Comments?.Count ?? 0,
            post.CreatedDateTime, post.Status,
            post.PostTags?.Select(pt => new TagResponse(pt.Tag.Id, pt.Tag.TagName, 0)).ToList(),
            post.Comments?.Select(c => new CommentResponse(c.Id, c.User?.FullName ?? "Unknown", c.CommentText, c.CreatedDateTime)).ToList());

        return Result<PostResponse>.Success(response,"پست مورد نظر");
    }
}

public record GetPostByIdQuery(Guid postId) : IRequest<Result<PostResponse>>;

public class GetPostByIdQueryValidator : AbstractValidator<GetPostByIdQuery>
{
    public GetPostByIdQueryValidator()
    {
        RuleFor(x => x.postId).NotEmpty().WithMessage("شناسه پست صحیح نیست")
            .Must(id => Guid.TryParse(id.ToString(),out _)).WithMessage("فرمت شناسه صحیح نیست");
    }
}