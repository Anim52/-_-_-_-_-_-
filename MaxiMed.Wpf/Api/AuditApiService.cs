using MaxiMed.Application.Audit;

namespace MaxiMed.Wpf.Api;

public sealed class AuditApiService : IAuditService
{
    private readonly ApiClient _api;
    public AuditApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<AuditLogDto>> SearchAsync(AuditSearchFilter filter, CancellationToken ct = default)
        => (await _api.PostAsync<IReadOnlyList<AuditLogDto>>("api/audit/search", filter, ct)) ?? Array.Empty<AuditLogDto>();
}
