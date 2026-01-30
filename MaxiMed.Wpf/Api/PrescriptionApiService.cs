using MaxiMed.Application.Visits;

namespace MaxiMed.Wpf.Api;

public sealed class PrescriptionApiService : IPrescriptionService
{
    private readonly ApiClient _api;
    public PrescriptionApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<PrescriptionDto>> GetAsync(long visitId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<PrescriptionDto>>($"api/visits/{visitId}/prescriptions", ct)) ?? Array.Empty<PrescriptionDto>();

    private sealed record AddReq(string Text);

    public async Task<long> AddAsync(long visitId, string text, CancellationToken ct = default)
        => (await _api.PostAsync<long>($"api/visits/{visitId}/prescriptions", new AddReq(text), ct))!;

    public Task DeleteAsync(long id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/visits/prescriptions/{id}", ct);
}
