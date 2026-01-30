using MaxiMed.Application.Patients;

namespace MaxiMed.Wpf.Api;

public sealed class PatientsApiService : IPatientService
{
    private readonly ApiClient _api;
    public PatientsApiService(ApiClient api) => _api = api;

    public Task<IReadOnlyList<PatientDto>?> SearchAsync(string? query, CancellationToken ct = default)
        => _api.GetAsync<IReadOnlyList<PatientDto>>($"api/patients/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct);

    public Task<PatientDto?> GetAsync(int id, CancellationToken ct = default)
        => _api.GetAsync<PatientDto?>($"api/patients/{id}", ct);

    public async Task<int> CreateAsync(PatientDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/patients", dto, ct))!;

    public Task UpdateAsync(PatientDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/patients", dto, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/patients/{id}", ct);

    async Task<IReadOnlyList<PatientDto>> IPatientService.SearchAsync(string? query, CancellationToken ct)
        => await SearchAsync(query, ct) ?? Array.Empty<PatientDto>();
}
