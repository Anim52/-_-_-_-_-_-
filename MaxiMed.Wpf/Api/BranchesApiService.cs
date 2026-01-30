using MaxiMed.Application.Common;

namespace MaxiMed.Wpf.Api;

public sealed class BranchesApiService : IBranchService
{
    private readonly ApiClient _api;
    public BranchesApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<BranchDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<BranchDto>>($"api/branches/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<BranchDto>();

    public async Task<int> CreateAsync(BranchDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/branches", dto, ct))!;

    public Task UpdateAsync(BranchDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/branches", dto, ct);

    public Task ArchiveAsync(int id, CancellationToken ct = default)
        => _api.PostAsync($"api/branches/{id}/archive", body: null, ct);
}
