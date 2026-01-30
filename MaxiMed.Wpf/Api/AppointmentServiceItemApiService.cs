using MaxiMed.Application.Appointments;

namespace MaxiMed.Wpf.Api;

public sealed class AppointmentServiceItemApiService : IAppointmentServiceItemService
{
    private readonly ApiClient _api;
    public AppointmentServiceItemApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<AppointmentServiceItemDto>> GetAsync(long appointmentId)
        => (await _api.GetAsync<IReadOnlyList<AppointmentServiceItemDto>>($"api/appointment-service-items/{appointmentId}")) ?? Array.Empty<AppointmentServiceItemDto>();

    private sealed record AddReq(int ServiceId, int Qty);

    public Task AddAsync(long appointmentId, int serviceId, int qty)
        => _api.PostAsync($"api/appointment-service-items/{appointmentId}", new AddReq(serviceId, qty));

    private sealed record UpdReq(long Id, int Qty, decimal Price);

    public Task UpdateAsync(long id, int qty, decimal price)
        => _api.PutAsync("api/appointment-service-items", new UpdReq(id, qty, price));

    public Task DeleteAsync(long id)
        => _api.DeleteAsync($"api/appointment-service-items/{id}");
}
