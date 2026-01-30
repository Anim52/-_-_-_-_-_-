using MaxiMed.Application.Patients;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/patient-history")]
public sealed class PatientHistoryController : ControllerBase
{
    private readonly IPatientHistoryService _svc;
    public PatientHistoryController(IPatientHistoryService svc) => _svc = svc;

    [HttpGet("{patientId:int}")]
    public async Task<ActionResult<PatientHistoryItemDto>> Get(int patientId, CancellationToken ct)
        => Ok(await _svc.GetAsync(patientId, ct));
}
