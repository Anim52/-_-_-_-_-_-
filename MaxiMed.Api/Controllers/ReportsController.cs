using MaxiMed.Application.Reports;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/reports")]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportService _svc;
    public ReportsController(IReportService svc) => _svc = svc;

    [HttpGet("visits")]
    public async Task<ActionResult<IReadOnlyList<VisitsReportRowDto>>> Visits([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
        => Ok(await _svc.GetVisitsAsync(from, to, ct));

    [HttpGet("revenue")]
    public async Task<ActionResult<IReadOnlyList<RevenueReportRowDto>>> Revenue([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
        => Ok(await _svc.GetRevenueAsync(from, to, ct));

    [HttpGet("doctors")]
    public async Task<ActionResult<IReadOnlyList<DoctorReportRowDto>>> Doctors([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
        => Ok(await _svc.GetDoctorsAsync(from, to, ct));

    [HttpGet("patient-stats")]
    public async Task<ActionResult<PatientStatsReportDto>> PatientStats(CancellationToken ct)
        => Ok(await _svc.GetPatientStatsAsync(ct));

    [HttpGet("gender-stats")]
    public async Task<ActionResult<IReadOnlyList<GenderStatRowDto>>> GenderStats(CancellationToken ct)
        => Ok(await _svc.GetGenderStatsAsync(ct));

    [HttpGet("age-stats")]
    public async Task<ActionResult<IReadOnlyList<AgeStatRowDto>>> AgeStats(CancellationToken ct)
        => Ok(await _svc.GetAgeStatsAsync(ct));

    [HttpGet("summary")]
    public async Task<ActionResult<SummaryReportDto>> Summary([FromQuery] DateTime from, [FromQuery] DateTime to, CancellationToken ct)
        => Ok(await _svc.GetSummaryAsync(from, to));
}
