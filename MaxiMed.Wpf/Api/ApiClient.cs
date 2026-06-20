using MaxiMed.Wpf.Services;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Api;

public sealed class ApiClient
{
    private readonly HttpClient _http;
    private readonly ISessionService _session;

    public ApiClient(HttpClient http, ISessionService session)
    {
        _http = http;
        _session = session;
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        ApplySessionHeaders();
        var res = await _http.GetAsync(url, ct);
        await EnsureOk(res);
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        ApplySessionHeaders();
        var res = await _http.PostAsJsonAsync(url, body, ct);
        await EnsureOk(res);
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task PostAsync(string url, object? body, CancellationToken ct = default)
    {
        ApplySessionHeaders();
        var res = await _http.PostAsJsonAsync(url, body, ct);
        await EnsureOk(res);
    }

    public async Task PutAsync(string url, object? body, CancellationToken ct = default)
    {
        ApplySessionHeaders();
        var res = await _http.PutAsJsonAsync(url, body, ct);
        await EnsureOk(res);
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        ApplySessionHeaders();
        var res = await _http.DeleteAsync(url, ct);
        await EnsureOk(res);
    }

    public async Task<bool> TryPingAsync(string url, CancellationToken ct = default)
    {
        try
        {
            ApplySessionHeaders();
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            var res = await _http.SendAsync(req, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private void ApplySessionHeaders()
    {
        _http.DefaultRequestHeaders.Clear();

        if (_session.CurrentUser != null)
        {
            _http.DefaultRequestHeaders.TryAddWithoutValidation(
                "X-User-Id",
                _session.CurrentUser.Id.ToString());
        }

        if (_session.DoctorId.HasValue)
        {
            _http.DefaultRequestHeaders.TryAddWithoutValidation(
                "X-Doctor-Id",
                _session.DoctorId.Value.ToString());
        }
    }

    private static async Task EnsureOk(HttpResponseMessage res)
    {
        if (res.IsSuccessStatusCode) return;

        var body = await res.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"API error {(int)res.StatusCode} {res.ReasonPhrase}: {body}");
    }
}
