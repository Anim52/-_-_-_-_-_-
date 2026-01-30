using MaxiMed.Application.Patients;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/patients")]
public sealed class PatientsController : ControllerBase
{
    private readonly IPatientService _svc;
    public PatientsController(IPatientService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<PatientDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PatientDto?>> Get(int id, CancellationToken ct)
        => Ok(await _svc.GetAsync(id, ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] PatientDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] PatientDto dto, CancellationToken ct)
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
