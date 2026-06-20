using MaxiMed.Application.Visits;
using MaxiMed.Domain.Entities;

namespace MaxiMed.Wpf.Api;

public sealed class VisitsApiService : IVisitService
{
    private readonly ApiClient _api;
    public VisitsApiService(ApiClient api) => _api = api;

    public async Task<VisitDto> GetOrCreateByAppointmentAsync(long appointmentId, int doctorId, CancellationToken ct = default)
        => (await _api.GetAsync<VisitDto>($"api/visits/by-appointment?appointmentId={appointmentId}&doctorId={doctorId}", ct))!;

    public Task SaveAsync(VisitDto dto, CancellationToken ct = default)
        => _api.PostAsync("api/visits/save", dto, ct);
    public Task<Visit?> GetByIdWithDetailsAsync(long id)
    {
        return _api.GetAsync<Visit>($"api/visits/{id}/print");
    }
}
