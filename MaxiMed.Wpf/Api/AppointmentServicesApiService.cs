using MaxiMed.Application.Appointments;

namespace MaxiMed.Wpf.Api;

public sealed class AppointmentServicesApiService : IAppointmentServicesService
{
    private readonly ApiClient _api;
    public AppointmentServicesApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AppointmentServiceItemDto>>($"api/appointment-services/{appointmentId}", ct)) ?? Array.Empty<AppointmentServiceItemDto>();

    private sealed record AddRequest(int ServiceId, int Qty, decimal? Price);

    public async Task<long> AddAsync(long appointmentId, int serviceId, int qty = 1, decimal? price = null, CancellationToken ct = default)
        => (await _api.PostAsync<long>($"api/appointment-services/{appointmentId}", new AddRequest(serviceId, qty, price), ct))!;

    public Task UpdateAsync(AppointmentServiceItemDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/appointment-services", dto, ct);

    public Task DeleteAsync(long id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/appointment-services/{id}", ct);
}
