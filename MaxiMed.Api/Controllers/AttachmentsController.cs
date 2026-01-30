using MaxiMed.Application.Attachments;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/attachments")]
public sealed class AttachmentsController : ControllerBase
{
    private readonly IAttachmentService _svc;
    public AttachmentsController(IAttachmentService svc) => _svc = svc;

    [HttpGet("by-patient/{patientId:int}")]
    public async Task<ActionResult<IReadOnlyList<AttachmentDto>>> ByPatient(int patientId, CancellationToken ct)
        => Ok(await _svc.GetByPatientAsync(patientId, ct));

    [HttpGet("by-visit/{visitId:long}")]
    public async Task<ActionResult<IReadOnlyList<AttachmentDto>>> ByVisit(long visitId, CancellationToken ct)
        => Ok(await _svc.GetByVisitAsync(visitId, ct));

    public sealed record UploadRequest(int PatientId, long? VisitId, string FileName, string? ContentType, byte[] Data);

    [HttpPost("upload")]
    public async Task<ActionResult<long>> Upload([FromBody] UploadRequest req, CancellationToken ct)
        => Ok(await _svc.UploadAsync(req.PatientId, req.VisitId, req.FileName, req.ContentType, req.Data, ct));

    public sealed record DownloadResponse(string FileName, string? ContentType, byte[] Data);

    [HttpGet("{id:long}/download")]
    public async Task<ActionResult<DownloadResponse>> Download(long id, CancellationToken ct)
    {
        var (fileName, contentType, data) = await _svc.DownloadAsync(id, ct);
        return Ok(new DownloadResponse(fileName, contentType, data));
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ct)
    {
        await _svc.DeleteAsync(id, ct);
        return NoContent();
    }
}
