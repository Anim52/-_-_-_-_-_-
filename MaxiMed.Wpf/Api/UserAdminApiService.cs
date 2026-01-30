using MaxiMed.Application.Users;

namespace MaxiMed.Wpf.Api;

public sealed class UserAdminApiService : IUserAdminService
{
    private readonly ApiClient _api;
    public UserAdminApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<UserListItemDto>>($"api/users/admin/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<UserListItemDto>();

    public Task<UserEditDto?> GetAsync(int id, CancellationToken ct = default)
        => _api.GetAsync<UserEditDto?>($"api/users/admin/{id}", ct);

    private sealed record Upsert(UserEditDto Dto, string? NewPassword);

    public async Task<int> CreateAsync(UserEditDto dto, string? newPassword, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/users/admin", new Upsert(dto, newPassword), ct))!;

    public Task UpdateAsync(UserEditDto dto, string? newPassword, CancellationToken ct = default)
        => _api.PutAsync("api/users/admin", new Upsert(dto, newPassword), ct);

    public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<RoleDto>>("api/users/roles", ct)) ?? Array.Empty<RoleDto>();

    public async Task<IReadOnlyList<string>> GetAllRoleNamesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<string>>("api/users/role-names", ct)) ?? Array.Empty<string>();
}
