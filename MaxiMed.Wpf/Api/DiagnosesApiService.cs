using MaxiMed.Application.Diagnoses;

namespace MaxiMed.Wpf.Api;

public sealed class DiagnosesApiService : IDiagnosisService
{
    private readonly ApiClient _api;
    public DiagnosesApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<DiagnosisDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<DiagnosisDto>>($"api/diagnoses/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<DiagnosisDto>();

    public async Task<int> CreateAsync(DiagnosisDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/diagnoses", dto, ct))!;

    public Task UpdateAsync(DiagnosisDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/diagnoses", dto, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/diagnoses/{id}", ct);
}
