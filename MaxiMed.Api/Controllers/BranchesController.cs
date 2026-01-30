using MaxiMed.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/branches")]
public sealed class BranchesController : ControllerBase
{
    private readonly IBranchService _svc;
    public BranchesController(IBranchService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<BranchDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] BranchDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] BranchDto dto, CancellationToken ct)
    {
        await _svc.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/archive")]
    public async Task<IActionResult> Archive(int id, CancellationToken ct)
    {
        await _svc.ArchiveAsync(id, ct);
        return NoContent();
    }
}
