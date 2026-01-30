using MaxiMed.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/specialties")]
public sealed class SpecialtiesController : ControllerBase
{
    private readonly ISpecialtyService _svc;
    public SpecialtiesController(ISpecialtyService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<SpecialtyDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] SpecialtyDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] SpecialtyDto dto, CancellationToken ct)
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
