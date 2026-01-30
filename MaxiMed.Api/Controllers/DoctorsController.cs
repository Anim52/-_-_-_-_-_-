using MaxiMed.Application.Common;
using MaxiMed.Application.Doctors;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/doctors")]
public sealed class DoctorsController : ControllerBase
{
    private readonly IDoctorService _svc;
    public DoctorsController(IDoctorService svc) => _svc = svc;

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<DoctorDto>>> Search([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchAsync(q, ct));

    [HttpGet("branches")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> Branches(CancellationToken ct)
        => Ok(await _svc.GetBranchesAsync(ct));

    [HttpGet("specialties")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> Specialties(CancellationToken ct)
        => Ok(await _svc.GetSpecialtiesAsync(ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] DoctorDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] DoctorDto dto, CancellationToken ct)
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
