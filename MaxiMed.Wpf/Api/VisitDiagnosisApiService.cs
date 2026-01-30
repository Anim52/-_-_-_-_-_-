using MaxiMed.Application.Visits;

namespace MaxiMed.Wpf.Api;

public sealed class VisitDiagnosisApiService : IVisitDiagnosisService
{
    private readonly ApiClient _api;
    public VisitDiagnosisApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<VisitDiagnosisItemDto>> GetAsync(long visitId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<VisitDiagnosisItemDto>>($"api/visits/{visitId}/diagnoses", ct)) ?? Array.Empty<VisitDiagnosisItemDto>();

    private sealed record Change(int DiagnosisId, bool IsPrimary);

    public Task AddAsync(long visitId, int diagnosisId, bool isPrimary = false, CancellationToken ct = default)
        => _api.PostAsync($"api/visits/{visitId}/diagnoses/add", new Change(diagnosisId, isPrimary), ct);

    public Task RemoveAsync(long visitId, int diagnosisId, CancellationToken ct = default)
        => _api.PostAsync($"api/visits/{visitId}/diagnoses/remove", new Change(diagnosisId, false), ct);

    public Task SetPrimaryAsync(long visitId, int diagnosisId, CancellationToken ct = default)
        => _api.PostAsync($"api/visits/{visitId}/diagnoses/set-primary", new Change(diagnosisId, true), ct);
}
