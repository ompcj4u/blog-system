using Application.Common;
using Application.Common.DTOs.Comments;
using Application.CQRS.Comment.Commands;
using Application.CQRS.Comment.Queries;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CommentsController(IMediator _mediator, ICurrentUserService _currentUser) : ControllerBase
{
    /// <summary>
    /// Add a comment to a post
    /// </summary>
    /// <param name="command">Comment details including post ID and text</param>
    /// <returns>Created comment details</returns>
    /// <response code="200">Comment added successfully</response>
    /// <response code="400">Validation failed</response>
    /// <response code="401">User not authenticated</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<CommentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentCommand command)
    {
        var commandWithUser = command with { UserId = _currentUser.UserId!.Value };
        var result = await _mediator.Send(commandWithUser);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Get all comments for a specific post
    /// </summary>
    /// <param name="postId">Post unique identifier</param>
    /// <returns>List of comments with author info</returns>
    /// <response code="200">Returns list of comments</response>
    [HttpGet("post/{postId:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<List<CommentResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCommentsByPost([FromRoute] Guid postId)
    {
        var result = await _mediator.Send(new GetCommentsByPostQuery(postId));
        return Ok(result);
    }
}
