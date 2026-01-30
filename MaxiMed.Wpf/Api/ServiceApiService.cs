using MaxiMed.Application.Services;

namespace MaxiMed.Wpf.Api;

public sealed class ServiceApiService : IServiceService
{
    private readonly ApiClient _api;
    public ServiceApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<ServiceDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<ServiceDto>>($"api/services/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<ServiceDto>();

    public async Task<int> CreateAsync(ServiceDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/services", dto, ct))!;

    public Task UpdateAsync(ServiceDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/services", dto, ct);

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/services/{id}", ct);
}
