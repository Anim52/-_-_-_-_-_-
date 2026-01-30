using MaxiMed.Application.Common;
using MaxiMed.Application.Doctors;

namespace MaxiMed.Wpf.Api;

public sealed class DoctorsApiService : IDoctorService
{
    private readonly ApiClient _api;
    public DoctorsApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<DoctorDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<DoctorDto>>($"api/doctors/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<DoctorDto>();

    public async Task<int> CreateAsync(DoctorDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/doctors", dto, ct))!;

    public Task UpdateAsync(DoctorDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/doctors", dto, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/doctors/{id}", ct);

    public async Task<IReadOnlyList<LookupItemDto>> GetBranchesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>("api/doctors/branches", ct)) ?? Array.Empty<LookupItemDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetSpecialtiesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>("api/doctors/specialties", ct)) ?? Array.Empty<LookupItemDto>();
}
