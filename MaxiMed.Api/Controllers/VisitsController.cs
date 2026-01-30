using MaxiMed.Application.Visits;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/visits")]
public sealed class VisitsController : ControllerBase
{
    private readonly IVisitService _visits;
    private readonly IVisitDiagnosisService _diag;
    private readonly IPrescriptionService _rx;

    public VisitsController(IVisitService visits, IVisitDiagnosisService diag, IPrescriptionService rx)
    {
        _visits = visits;
        _diag = diag;
        _rx = rx;
    }

    [HttpGet("by-appointment")]
    public async Task<ActionResult<VisitDto>> GetOrCreateByAppointment([FromQuery] long appointmentId, [FromQuery] int doctorId, CancellationToken ct)
        => Ok(await _visits.GetOrCreateByAppointmentAsync(appointmentId, doctorId, ct));

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] VisitDto dto, CancellationToken ct)
    {
        await _visits.SaveAsync(dto, ct);
        return NoContent();
    }

    // diagnoses for visit
    [HttpGet("{visitId:long}/diagnoses")]
    public async Task<ActionResult<IReadOnlyList<VisitDiagnosisItemDto>>> Diagnoses(long visitId, CancellationToken ct)
        => Ok(await _diag.GetAsync(visitId, ct));

    public sealed record VisitDiagnosisChange(int DiagnosisId, bool IsPrimary);

    [HttpPost("{visitId:long}/diagnoses/add")]
    public async Task<IActionResult> AddDiagnosis(long visitId, [FromBody] VisitDiagnosisChange req, CancellationToken ct)
    {
        await _diag.AddAsync(visitId, req.DiagnosisId, req.IsPrimary, ct);
        return NoContent();
    }

    [HttpPost("{visitId:long}/diagnoses/remove")]
    public async Task<IActionResult> RemoveDiagnosis(long visitId, [FromBody] VisitDiagnosisChange req, CancellationToken ct)
    {
        await _diag.RemoveAsync(visitId, req.DiagnosisId, ct);
        return NoContent();
    }

    [HttpPost("{visitId:long}/diagnoses/set-primary")]
    public async Task<IActionResult> SetPrimary(long visitId, [FromBody] VisitDiagnosisChange req, CancellationToken ct)
    {
        await _diag.SetPrimaryAsync(visitId, req.DiagnosisId, ct);
        return NoContent();
    }

    // prescriptions
    [HttpGet("{visitId:long}/prescriptions")]
    public async Task<ActionResult<IReadOnlyList<PrescriptionDto>>> Prescriptions(long visitId, CancellationToken ct)
        => Ok(await _rx.GetAsync(visitId, ct));

    public sealed record AddPrescriptionRequest(string Text);

    [HttpPost("{visitId:long}/prescriptions")]
    public async Task<ActionResult<long>> AddPrescription(long visitId, [FromBody] AddPrescriptionRequest req, CancellationToken ct)
        => Ok(await _rx.AddAsync(visitId, req.Text, ct));

    [HttpDelete("prescriptions/{id:long}")]
    public async Task<IActionResult> DeletePrescription(long id, CancellationToken ct)
    {
        await _rx.DeleteAsync(id, ct);
        return NoContent();
    }
}
