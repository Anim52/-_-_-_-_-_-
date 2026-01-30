using MaxiMed.Application.Audit;
using Microsoft.AspNetCore.Mvc;

namespace MaxiMed.Api.Controllers;

[ApiController]
[Route("api/audit")]
public sealed class AuditController : ControllerBase
{
    private readonly IAuditService _svc;
    public AuditController(IAuditService svc) => _svc = svc;

    [HttpPost("search")]
    public async Task<ActionResult<IReadOnlyList<AuditLogDto>>> Search([FromBody] AuditSearchFilter filter, CancellationToken ct)
        => Ok(await _svc.SearchAsync(filter, ct));
}
