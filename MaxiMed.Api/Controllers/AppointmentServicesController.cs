using MaxiMed.Application.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/appointment-services")]
public sealed class AppointmentServicesController : ControllerBase
{
    private readonly IAppointmentServicesService _svc;
    public AppointmentServicesController(IAppointmentServicesService svc) => _svc = svc;

    [HttpGet("{appointmentId:long}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentServiceItemDto>>> Get(long appointmentId, CancellationToken ct)
        => Ok(await _svc.GetAsync(appointmentId, ct));

    public sealed record AddRequest(int ServiceId, int Qty, decimal? Price);

    [HttpPost("{appointmentId:long}")]
    public async Task<ActionResult<long>> Add(long appointmentId, [FromBody] AddRequest req, CancellationToken ct)
        => Ok(await _svc.AddAsync(appointmentId, req.ServiceId, req.Qty, req.Price, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AppointmentServiceItemDto dto, CancellationToken ct)
    {
        await _svc.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
