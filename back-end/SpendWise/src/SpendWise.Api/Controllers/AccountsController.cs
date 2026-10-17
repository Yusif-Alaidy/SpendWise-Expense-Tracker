using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Accounts;

namespace SpendWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
/// <summary>
/// Manage accounts: list, get, create, update and delete.
/// </summary>
public class AccountsController : ControllerBase
{
    private readonly IAccountService _service;

    public AccountsController(IAccountService service)
    {
        _service = service;
    }

    /// <summary>
    /// Get all accounts for the current user.
    /// </summary>
    /// <response code="200">List of accounts.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<AccountDto>), StatusCodes.Status200OK)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _service.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    /// <summary>
    /// Get account by id.
    /// </summary>
    /// <response code="200">Account found.</response>
    /// <response code="404">Account not found. Returns ProblemDetails.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> Get(int id, CancellationToken cancellationToken)
    {
        var item = await _service.GetByIdAsync(id, cancellationToken);
        return Ok(item);
    }

    /// <summary>
    /// Create a new account.
    /// </summary>
    /// <response code="201">Account created.</response>
    /// <response code="400">Validation failed. Returns ProblemDetails.</response>
    [HttpPost]
    [ProducesResponseType(typeof(AccountDto), StatusCodes.Status201Created)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var created = await _service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing account.
    /// </summary>
    /// <response code="204">Update successful.</response>
    /// <response code="404">Account not found. Returns ProblemDetails.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        await _service.UpdateAsync(id, request, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Delete an account.
    /// </summary>
    /// <response code="204">Deletion successful.</response>
    /// <response code="404">Account not found. Returns ProblemDetails.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await _service.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
