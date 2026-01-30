using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _svc;
    public AppointmentsController(IAppointmentService svc) => _svc = svc;

    [HttpGet("day")]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetDay([FromQuery] DateTime day, [FromQuery] int? doctorId, CancellationToken ct)
        => Ok(await _svc.GetDayAsync(day, doctorId, ct));

    [HttpGet("by-patient/{patientId:int}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> ByPatient(int patientId, CancellationToken ct)
        => Ok(await _svc.GetByPatientAsync(patientId, ct));

    [HttpGet("patient-lookup/{patientId:int}")]
    public async Task<ActionResult<LookupItemDto?>> PatientLookup(int patientId, CancellationToken ct)
        => Ok(await _svc.GetPatientLookupAsync(patientId, ct));

    [HttpGet("active-doctors")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> ActiveDoctors(CancellationToken ct)
        => Ok(await _svc.GetActiveDoctorsAsync(ct));

    [HttpGet("active-branches")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> ActiveBranches(CancellationToken ct)
        => Ok(await _svc.GetActiveBranchesAsync(ct));

    [HttpGet("active-specialties")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> ActiveSpecialties(CancellationToken ct)
        => Ok(await _svc.GetActiveSpecialtiesAsync(ct));

    [HttpGet("doctors-by-specialty/{specialtyId:int}")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> DoctorsBySpecialty(int specialtyId, CancellationToken ct)
        => Ok(await _svc.GetDoctorsBySpecialtyAsync(specialtyId, ct));

    [HttpGet("search-patients")]
    public async Task<ActionResult<IReadOnlyList<LookupItemDto>>> SearchPatients([FromQuery] string? q, CancellationToken ct)
        => Ok(await _svc.SearchPatientsAsync(q, ct));

    [HttpPost("{id:int}/complete")]
    public async Task<IActionResult> Complete(int id, CancellationToken ct)
    {
        await _svc.CompleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        await _svc.CancelAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{appointmentId:long}/patient-id")]
    public async Task<ActionResult<int>> GetPatientId(long appointmentId, CancellationToken ct)
        => Ok(await _svc.GetPatientIdByAppointmentAsync(appointmentId, ct));

    [HttpGet("free-slots")]
    public async Task<ActionResult<IReadOnlyList<FreeSlotDto>>> FreeSlots([FromQuery] int doctorId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int maxResults, CancellationToken ct)
        => Ok(await _svc.FindFreeSlotsAsync(doctorId, fromDate, toDate, maxResults, ct));

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AppointmentDto dto, CancellationToken ct)
        => Ok(await _svc.CreateAsync(dto, ct));

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AppointmentDto dto, CancellationToken ct)
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
