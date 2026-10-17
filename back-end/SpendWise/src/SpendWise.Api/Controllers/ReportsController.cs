using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendWise.Application.Reports.Dtos;
using SpendWise.Application.Reports;

namespace SpendWise.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly IReportService _service;

    public ReportsController(IReportService service)
    {
        _service = service;
    }

    [HttpGet("monthly-summary")]
    [ProducesResponseType(typeof(MonthlySummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> MonthlySummary([FromQuery] MonthlySummaryQueryParams query, CancellationToken cancellationToken)
    {
        var res = await _service.GetMonthlySummaryAsync(query, cancellationToken);
        return Ok(res);
    }

    [HttpGet("category-breakdown")]
    [ProducesResponseType(typeof(CategoryBreakdownDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<IActionResult> CategoryBreakdown([FromQuery] CategoryBreakdownQueryParams query, CancellationToken cancellationToken)
    {
        var res = await _service.GetCategoryBreakdownAsync(query, cancellationToken);
        return Ok(res);
    }
}
