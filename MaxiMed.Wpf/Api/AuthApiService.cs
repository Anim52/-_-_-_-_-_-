using MaxiMed.Application.Auth;
using MaxiMed.Domain.Entities;

namespace MaxiMed.Wpf.Api;

public sealed class AuthApiService : IAuthService
{
    private readonly ApiClient _api;
    public AuthApiService(ApiClient api) => _api = api;

    private sealed record LoginRequest(string Login, string Password);

    public Task<User?> LoginAsync(string login, string password)
        => _api.PostAsync<User?>("api/auth/login", new LoginRequest(login, password));
}
