using Application.Common;
using Application.Common.DTOs.Posts;
using Application.CQRS.Post.Commands;
using Application.CQRS.Post.Queries;
using Application.CQRS.PostLike.Commands;
using Application.Interfaces;
using Asp.Versioning;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Security.Claims;

namespace Api.Controllers;
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class PostsController(IMediator _mediator, ICurrentUserService _currentUser) : ControllerBase
{
    /// <summary>
    /// Get all published posts with pagination
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Items per page (default: 10, max: 50)</param>
    /// <returns>Paginated list of published posts with author and counts</returns>
    /// <response code="200">Returns paginated list of posts</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PaginatedResult<PostResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPublishedPosts(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageSize > 50) pageSize = 50;
        var result = await _mediator.Send(new GetPublishedPostsQuery(pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Get a specific post by its ID with full details
    /// </summary>
    /// <param name="id">Post unique identifier (GUID)</param>
    /// <returns>Post with author, tags, comments and like count</returns>
    /// <response code="200">Returns post details</response>
    /// <response code="404">Post not found</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPostById([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new GetPostByIdQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    /// <summary>
    /// Get all published posts with a specific tag
    /// </summary>
    /// <param name="tagId">Tag unique identifier (GUID)</param>
    /// <returns>List of posts with specified tag</returns>
    /// <response code="200">Returns filtered posts</response>
    [HttpGet("tag/{tagId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<List<PostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsByTag([FromRoute] Guid tagId)
    {
        var result = await _mediator.Send(new GetPostsByTagQuery(tagId));
        return Ok(result);
    }

    /// <summary>
    /// Get all posts by a specific author
    /// </summary>
    /// <param name="authorId">Author unique identifier (GUID)</param>
    /// <returns>List of posts by specified author</returns>
    /// <response code="200">Returns author's posts</response>
    [HttpGet("author/{authorId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<List<PostResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPostsByAuthor([FromRoute] Guid authorId)
    {
        var result = await _mediator.Send(new GetPostsByAuthorQuery(authorId));
        return Ok(result);
    }

    /// <summary>
    /// Get current user's posts by status
    /// </summary>
    /// <param name="status">Post status (Draft, Published, Archived)</param>
    /// <returns>List of current user's posts with specified status</returns>
    /// <response code="200">Returns filtered posts</response>
    /// <response code="401">User not authenticated</response>
    [HttpGet("my/{status}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<List<PostResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyPostsByStatus([FromRoute] PostStatus status)
    {
        var result = await _mediator.Send(new GetPostsByStatusQuery(_currentUser.UserId!.Value, status));
        return Ok(result);
    }

    /// <summary>
    /// Create a new blog post
    /// </summary>
    /// <param name="command">Post details including title, content and tag IDs</param>
    /// <returns>Created post details</returns>
    /// <response code="201">Post created successfully</response>
    /// <response code="400">Validation failed or invalid data</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<PostResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest command)
    {

        var result = await _mediator.Send(new CreatePostCommand(command.Title, command.Content, command.Tags, _currentUser.UserId!.Value));

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(GetPostById), new { id = result.Value!.Id }, result);
    }

    /// <summary>
    /// Update an existing post
    /// </summary>
    /// <param name="id">Post unique identifier (GUID)</param>
    /// <param name="command">Updated post data including title, content, status and tags</param>
    /// <returns>Updated post details</returns>
    /// <response code="200">Post updated successfully</response>
    /// <response code="400">Validation failed</response>
    /// <response code="404">Post not found</response>
    /// <response code="401">User not authenticated</response>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<PostResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdatePost([FromRoute] Guid id, [FromBody] UpdatePostCommand command)
    {
        var commandWithId = command with { postId = id };
        var result = await _mediator.Send(commandWithId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Add a tag to an existing post
    /// </summary>
    /// <param name="id">Post unique identifier (GUID)</param>
    /// <param name="command">Tag ID to add</param>
    /// <returns>Success confirmation</returns>
    /// <response code="200">Tag added successfully</response>
    /// <response code="400">Tag already exists or invalid data</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("{id:guid}/tags")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddTagToPost([FromRoute] Guid id, [FromBody] AddTagToPostCommand command)
    {
        var commandWithPostId = command with { PostId = id };
        var result = await _mediator.Send(commandWithPostId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Toggle like/unlike on a post
    /// </summary>
    /// <param name="id">Post unique identifier (GUID)</param>
    /// <returns>Current like status (true = liked, false = unliked)</returns>
    /// <response code="200">Toggle successful</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost("{id:guid}/like")]
    [Authorize]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ToggleLike([FromRoute] Guid id)
    {
        var result = await _mediator.Send(new ToggleLikeCommand(id, _currentUser.UserId!.Value));
        return Ok(result);
    }
}