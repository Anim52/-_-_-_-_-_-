using MaxiMed.Application.Services;

namespace MaxiMed.Wpf.Api;

public sealed class DoctorDayOffApiService : IDoctorDayOffService
{
    private readonly ApiClient _api;
    public DoctorDayOffApiService(ApiClient api) => _api = api;

    private sealed record AddReq(int DoctorId, DateTime Date, string? Reason);

    public Task AddDayOffAsync(int doctorId, DateTime date, string? reason, CancellationToken ct = default)
        => _api.PostAsync("api/doctor-dayoff/add", new AddReq(doctorId, date, reason), ct);

    public async Task<bool> IsDayOffAsync(int doctorId, DateTime date, CancellationToken ct = default)
        => (await _api.GetAsync<bool>($"api/doctor-dayoff/is-dayoff?doctorId={doctorId}&date={Uri.EscapeDataString(date.ToString("O"))}", ct))!;
}
