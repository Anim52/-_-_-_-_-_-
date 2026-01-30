using MaxiMed.Application.Appointments;
using MaxiMed.Application.Common;

namespace MaxiMed.Wpf.Api;

public sealed class AppointmentsApiService : IAppointmentService
{
    private readonly ApiClient _api;
    public AppointmentsApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<AppointmentDto>> GetDayAsync(DateTime day, int? doctorId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AppointmentDto>>($"api/appointments/day?day={Uri.EscapeDataString(day.ToString("O"))}&doctorId={(doctorId?.ToString() ?? string.Empty)}", ct))
           ?? Array.Empty<AppointmentDto>();

    public Task<LookupItemDto?> GetPatientLookupAsync(int patientId, CancellationToken ct = default)
        => _api.GetAsync<LookupItemDto?>($"api/appointments/patient-lookup/{patientId}", ct);

    public async Task<IReadOnlyList<LookupItemDto>> GetActiveDoctorsAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>("api/appointments/active-doctors", ct)) ?? Array.Empty<LookupItemDto>();

    public async Task<IReadOnlyList<AppointmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AppointmentDto>>($"api/appointments/by-patient/{patientId}", ct)) ?? Array.Empty<AppointmentDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetActiveBranchesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>("api/appointments/active-branches", ct)) ?? Array.Empty<LookupItemDto>();

    public async Task<IReadOnlyList<LookupItemDto>> SearchPatientsAsync(string? query, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>($"api/appointments/search-patients?q={Uri.EscapeDataString(query ?? string.Empty)}", ct)) ?? Array.Empty<LookupItemDto>();

    public Task CompleteAsync(int id, CancellationToken ct = default)
        => _api.PostAsync($"api/appointments/{id}/complete", body: null, ct);

    public Task CancelAsync(int id, CancellationToken ct = default)
        => _api.PostAsync($"api/appointments/{id}/cancel", body: null, ct);

    public async Task<IReadOnlyList<FreeSlotDto>> FindFreeSlotsAsync(int doctorId, DateTime fromDate, DateTime toDate, int maxResults = 20, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<FreeSlotDto>>(
                $"api/appointments/free-slots?doctorId={doctorId}&fromDate={Uri.EscapeDataString(fromDate.ToString("O"))}&toDate={Uri.EscapeDataString(toDate.ToString("O"))}&maxResults={maxResults}",
                ct)) ?? Array.Empty<FreeSlotDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetActiveSpecialtiesAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>("api/appointments/active-specialties", ct)) ?? Array.Empty<LookupItemDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<LookupItemDto>>($"api/appointments/doctors-by-specialty/{specialtyId}", ct)) ?? Array.Empty<LookupItemDto>();

    public async Task<int> GetPatientIdByAppointmentAsync(long appointmentId, CancellationToken ct = default)
        => (await _api.GetAsync<int>($"api/appointments/{appointmentId}/patient-id", ct))!;

    public async Task<int> CreateAsync(AppointmentDto dto, CancellationToken ct = default)
        => (await _api.PostAsync<int>("api/appointments", dto, ct))!;

    public Task DeleteAsync(int id, CancellationToken ct = default)
        => _api.DeleteAsync($"api/appointments/{id}", ct);

    public Task UpdateAsync(AppointmentDto dto, CancellationToken ct = default)
        => _api.PutAsync("api/appointments", dto, ct);
}
