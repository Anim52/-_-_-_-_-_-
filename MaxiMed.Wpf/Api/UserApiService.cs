using MaxiMed.Application.Users;

namespace MaxiMed.Wpf.Api;

public sealed class UserApiService : IUserService
{
    private readonly ApiClient _api;
    public UserApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<UserListItemDto>>($"api/users/search?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<UserListItemDto>();
}
