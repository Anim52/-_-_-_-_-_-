using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;
using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/appointments")]
public sealed class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _svc;
    private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

    public AppointmentsController(IAppointmentService svc, IDbContextFactory<MaxiMedDbContext> dbFactory)
    {
        _svc = svc;
        _dbFactory = dbFactory;
    }

    [HttpGet("day")]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> GetDay([FromQuery] DateTime day, [FromQuery] int? doctorId, CancellationToken ct)
    {
        var doctorScope = await GetCurrentDoctorScopeAsync(ct);
        var effectiveDoctorId = doctorScope ?? doctorId;

        return Ok(await _svc.GetDayAsync(day.Date, effectiveDoctorId, ct));
    }

    [HttpGet("by-patient/{patientId:int}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> ByPatient(int patientId, CancellationToken ct)
    {
        var doctorScope = await GetCurrentDoctorScopeAsync(ct);

        if (doctorScope is > 0)
            return Ok(await _svc.GetByPatientForDoctorAsync(patientId, doctorScope.Value, ct));

        if (doctorScope is <= 0)
            return Ok(Array.Empty<AppointmentDto>());

        return Ok(await _svc.GetByPatientAsync(patientId, ct));
    }

    [HttpGet("by-patient-for-doctor/{patientId:int}/{doctorId:int}")]
    public async Task<ActionResult<IReadOnlyList<AppointmentDto>>> ByPatientForDoctor(int patientId, int doctorId, CancellationToken ct)
    {
        var doctorScope = await GetCurrentDoctorScopeAsync(ct);
        var effectiveDoctorId = doctorScope ?? doctorId;

        if (effectiveDoctorId <= 0)
            return Ok(Array.Empty<AppointmentDto>());

        return Ok(await _svc.GetByPatientForDoctorAsync(patientId, effectiveDoctorId, ct));
    }

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
        if (!await CanAccessAppointmentAsync(id, ct))
            return Forbid();

        await _svc.CompleteAsync(id, ct);
        return NoContent();
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct)
    {
        if (!await CanAccessAppointmentAsync(id, ct))
            return Forbid();

        await _svc.CancelAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{appointmentId:long}/patient-id")]
    public async Task<ActionResult<int>> GetPatientId(long appointmentId, CancellationToken ct)
    {
        if (!await CanAccessAppointmentAsync(appointmentId, ct))
            return Forbid();

        return Ok(await _svc.GetPatientIdByAppointmentAsync(appointmentId, ct));
    }

    [HttpGet("free-slots")]
    public async Task<ActionResult<IReadOnlyList<FreeSlotDto>>> FreeSlots([FromQuery] int doctorId, [FromQuery] DateTime fromDate, [FromQuery] DateTime toDate, [FromQuery] int maxResults, CancellationToken ct)
    {
        var doctorScope = await GetCurrentDoctorScopeAsync(ct);
        var effectiveDoctorId = doctorScope ?? doctorId;

        if (effectiveDoctorId <= 0)
            return Ok(Array.Empty<FreeSlotDto>());

        return Ok(await _svc.FindFreeSlotsAsync(effectiveDoctorId, fromDate.Date, toDate.Date, maxResults, ct));
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create([FromBody] AppointmentDto dto, CancellationToken ct)
    {
        if (!CanUseDoctor(dto.DoctorId, await GetCurrentDoctorScopeAsync(ct)))
            return Forbid();

        return Ok(await _svc.CreateAsync(dto, ct));
    }

    [HttpPut]
    public async Task<IActionResult> Update([FromBody] AppointmentDto dto, CancellationToken ct)
    {
        var doctorScope = await GetCurrentDoctorScopeAsync(ct);

        if (!CanUseDoctor(dto.DoctorId, doctorScope))
            return Forbid();

        if (dto.Id > 0 && !await CanAccessAppointmentAsync(dto.Id, doctorScope, ct))
            return Forbid();

        await _svc.UpdateAsync(dto, ct);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        if (!await CanAccessAppointmentAsync(id, ct))
            return Forbid();

        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpGet("{id:long}/ticket")]
    public async Task<IActionResult> GetTicket(long id, CancellationToken ct)
    {
        if (!await CanAccessAppointmentAsync(id, ct))
            return Forbid();

        var appointment = await _svc.GetByIdWithDetailsAsync(id);

        if (appointment == null)
            return NotFound();

        return Ok(new
        {
            PatientName = ((appointment.Patient.LastName + " " +
                           appointment.Patient.FirstName + " " +
                           (appointment.Patient.MiddleName ?? "")).Trim()),
            DoctorName = appointment.Doctor.FullName,
            BranchName = appointment.Branch.Name,
            Room = appointment.Doctor.Room,
            StartAt = appointment.StartAt,
            EndAt = appointment.EndAt
        });
    }

    private async Task<int?> GetCurrentDoctorScopeAsync(CancellationToken ct)
    {
        if (!Request.Headers.TryGetValue("X-User-Id", out var headerValues) ||
            !int.TryParse(headerValues.FirstOrDefault(), out var userId))
        {
            return null;
        }

        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var user = await db.Users.AsNoTracking()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId && u.IsActive, ct);

        if (user is null)
            return null;

        var isDoctor = user.UserRoles.Any(ur =>
            ur.Role != null &&
            string.Equals(ur.Role.Name, "Doctor", StringComparison.OrdinalIgnoreCase));

        if (!isDoctor)
            return null;

        // Если пользователь имеет роль Doctor, но не привязан к врачу, он не должен видеть чужие записи.
        return user.DoctorId ?? -1;
    }

    private static bool CanUseDoctor(int requestedDoctorId, int? doctorScope)
    {
        if (doctorScope is null)
            return true;

        return doctorScope > 0 && requestedDoctorId == doctorScope.Value;
    }

    private async Task<bool> CanAccessAppointmentAsync(long appointmentId, CancellationToken ct)
        => await CanAccessAppointmentAsync(appointmentId, await GetCurrentDoctorScopeAsync(ct), ct);

    private async Task<bool> CanAccessAppointmentAsync(long appointmentId, int? doctorScope, CancellationToken ct)
    {
        if (doctorScope is null)
            return true;

        if (doctorScope <= 0)
            return false;

        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        return await db.Appointments.AsNoTracking()
            .AnyAsync(a => a.Id == appointmentId && a.DoctorId == doctorScope.Value, ct);
    }
}
