using MaxiMed.Application.Diagnoses;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/diagnoses")]
public sealed class DiagnosesController : ControllerBase
{
    private readonly IDiagnosisService _svc;
    public DiagnosesController(IDiagnosisService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<DiagnosisDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] DiagnosisDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DiagnosisDto dto, CancellationToken ct)
    {
        await _svc.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
