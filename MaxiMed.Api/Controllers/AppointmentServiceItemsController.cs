using MaxiMed.Application.Appointments;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/appointment-service-items")]
public sealed class AppointmentServiceItemsController : ControllerBase
{
    private readonly IAppointmentServiceItemService _svc;
    public AppointmentServiceItemsController(IAppointmentServiceItemService svc) => _svc = svc;

    [HttpGet("{appointmentId:long}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentServiceItemDto>>> Get(long appointmentId)
        => Ok(await _svc.GetAsync(appointmentId));

    public sealed record AddRequest(int ServiceId, int Qty);

    [HttpPost("{appointmentId:long}")]
    public async Task<IActionResult> Add(long appointmentId, [FromBody] AddRequest req)
    {
        await _svc.AddAsync(appointmentId, req.ServiceId, req.Qty);
        return NoContent();
    }

    public sealed record UpdateRequest(long Id, int Qty, decimal Price);

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateRequest req)
    {
        await _svc.UpdateAsync(req.Id, req.Qty, req.Price);
        return NoContent();
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _svc.DeleteAsync(id);
        return NoContent();
    }
}
