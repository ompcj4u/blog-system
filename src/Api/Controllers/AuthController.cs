using Application.Common;
using Application.Common.DTOs.Auth;
using Application.CQRS.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Api.Controllers;
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class AuthController(IMediator _mediator) : ControllerBase
{
    /// <summary>
    /// Register a new user account
    /// </summary>
    /// <param name="command">User registration details including fullname, email and password</param>
    /// <returns>JWT access token and refresh token</returns>
    /// <response code="200">Registration successful - returns tokens</response>
    /// <response code="400">Registration failed - validation errors or duplicate email</response>
    [HttpPost("register")]
    [EnableRateLimiting("Auth")]
    [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterUserCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Login with existing account
    /// </summary>
    /// <param name="command">Login credentials (email and password)</param>
    /// <returns>JWT access token and refresh token</returns>
    /// <response code="200">Login successful - returns tokens</response>
    /// <response code="400">Login failed - invalid credentials</response>
    [HttpPost("login")]
    [EnableRateLimiting("Auth")]
    [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    /// <summary>
    /// Refresh expired access token using refresh token
    /// </summary>
    /// <param name="command">Expired access token and valid refresh token</param>
    /// <returns>New JWT access token and refresh token pair</returns>
    /// <response code="200">Token refreshed successfully</response>
    /// <response code="400">Invalid or expired refresh token</response>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(Result<TokenResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}


