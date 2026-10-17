using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Auth;
using SpendWise.Application.Common.Interfaces;

namespace SpendWise.Api.Controllers;

/// <summary>
/// Authentication endpoints for registering, logging in and retrieving the current user.
/// Register and Login are anonymous; other endpoints require a valid JWT.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUserService _currentUser;

    public AuthController(IAuthService auth, ICurrentUserService currentUser)
    {
        _auth = auth;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Register a new user.
    /// </summary>
    /// <remarks>
    /// Creates a new user account and returns the created user information.
    /// </remarks>
    /// <param name="request">Registration request containing name, email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="201">User created successfully.</response>
    /// <response code="400">Invalid request (validation errors). Returns ProblemDetails.</response>
    /// <response code="409">Conflict (email already in use). Returns ProblemDetails.</response>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, result);
    }

    /// <summary>
    /// Authenticate a user and return a JWT.
    /// </summary>
    /// <param name="request">Login request with email and password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <response code="200">Authentication successful. Returns JWT and user info.</response>
    /// <response code="400">Invalid credentials. Returns ProblemDetails.</response>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var result = await _auth.LoginAsync(request, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Get information about the current authenticated user.
    /// </summary>
    /// <response code="200">Returns current user's id, name and email.</response>
    /// <response code="401">Unauthorized when no valid JWT is provided. Returns ProblemDetails.</response>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var id = _currentUser.UserId;
        var name = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
        var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
        return Ok(new { Id = id, Name = name, Email = email });
    }
}
