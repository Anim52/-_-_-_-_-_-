using System.Net.Http.Json;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;


namespace MaxiMed.Wpf.Api;

public sealed class ApiClient
{
    private readonly HttpClient _http;

    public ApiClient(HttpClient http) => _http = http;

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var res = await _http.GetAsync(url, ct);
        await EnsureOk(res);
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task<T?> PostAsync<T>(string url, object? body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(url, body, ct);
        await EnsureOk(res);
        return await res.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task PostAsync(string url, object? body, CancellationToken ct = default)
    {
        var res = await _http.PostAsJsonAsync(url, body, ct);
        await EnsureOk(res);
    }

    public async Task PutAsync(string url, object? body, CancellationToken ct = default)
    {
        var res = await _http.PutAsJsonAsync(url, body, ct);
        await EnsureOk(res);
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var res = await _http.DeleteAsync(url, ct);
        await EnsureOk(res);
    }

    private static async Task EnsureOk(HttpResponseMessage res)
    {
        if (res.IsSuccessStatusCode) return;

        var body = await res.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"API error {(int)res.StatusCode} {res.ReasonPhrase}: {body}");
    }


    public async Task<bool> TryPingAsync(string url, CancellationToken ct = default)
    {
        try
        {
            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            var res = await _http.SendAsync(req, ct);
            return true; // reachable (even if 404)
        }
        catch
        {
            return false;
        }
    }
}
