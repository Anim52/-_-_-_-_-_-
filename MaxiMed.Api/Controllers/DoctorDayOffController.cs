using MaxiMed.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/doctor-dayoff")]
public sealed class DoctorDayOffController : ControllerBase
{
    private readonly IDoctorDayOffService _svc;
    public DoctorDayOffController(IDoctorDayOffService svc) => _svc = svc;

    public sealed record AddRequest(int DoctorId, DateTime Date, string? Reason);

    [HttpPost("add")]
    public async Task<IActionResult> Add([FromBody] AddRequest req, CancellationToken ct)
    {
        await _svc.AddDayOffAsync(req.DoctorId, req.Date, req.Reason, ct);
        return NoContent();
    }

    [HttpGet("is-dayoff")]
    public async Task<ActionResult<bool>> IsDayOff([FromQuery] int doctorId, [FromQuery] DateTime date, CancellationToken ct)
        => Ok(await _svc.IsDayOffAsync(doctorId, date, ct));
}
