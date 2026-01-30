using MaxiMed.Application.Common;

namespace MaxiMed.Wpf.Api;

public sealed class SpecialtiesApiService : ISpecialtyService
{
    private readonly ApiClient _api;
    public SpecialtiesApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<SpecialtyDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<SpecialtyDto>>($"api/specialties/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<SpecialtyDto>();

    public async Task<int> CreateAsync(SpecialtyDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/specialties", dto, ct))!;

    public Task UpdateAsync(SpecialtyDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/specialties", dto, ct);

    public Task ArchiveAsync(int id, CancellationToken ct = default)
        => _api.PostAsync($"api/specialties/{id}/archive", body: null, ct);
}
