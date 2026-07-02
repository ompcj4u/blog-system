using Application.Common;
using Application.Common.DTOs.Tags;
using Application.CQRS.Tag.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Api.Controllers;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TagsController(IMediator _mediator) : ControllerBase
{
    /// <summary>
    /// Get all available tags
    /// </summary>
    /// <returns>List of all tags with post count</returns>
    /// <response code="200">Returns list of tags</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Result<List<TagResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllTags()
    {
        var result = await _mediator.Send(new GetAllTagsQuery());
        return Ok(result);
    }
}
