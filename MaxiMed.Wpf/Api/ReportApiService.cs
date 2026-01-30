using MaxiMed.Application.Reports;

namespace MaxiMed.Wpf.Api;

public sealed class ReportApiService : IReportService
{
    private readonly ApiClient _api;
    public ReportApiService(ApiClient api) => _api = api;

    public async Task<IReadOnlyList<VisitsReportRowDto>> GetVisitsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<VisitsReportRowDto>>($"api/reports/visits?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}", ct)) ?? Array.Empty<VisitsReportRowDto>();

    public async Task<IReadOnlyList<RevenueReportRowDto>> GetRevenueAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<RevenueReportRowDto>>($"api/reports/revenue?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}", ct)) ?? Array.Empty<RevenueReportRowDto>();

    public async Task<IReadOnlyList<DoctorReportRowDto>> GetDoctorsAsync(DateTime from, DateTime to, CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<DoctorReportRowDto>>($"api/reports/doctors?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}", ct)) ?? Array.Empty<DoctorReportRowDto>();

    public async Task<PatientStatsReportDto> GetPatientStatsAsync(CancellationToken ct = default)
        => (await _api.GetAsync<PatientStatsReportDto>("api/reports/patient-stats", ct))!;

    public async Task<IReadOnlyList<GenderStatRowDto>> GetGenderStatsAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<GenderStatRowDto>>("api/reports/gender-stats", ct)) ?? Array.Empty<GenderStatRowDto>();

    public async Task<IReadOnlyList<AgeStatRowDto>> GetAgeStatsAsync(CancellationToken ct = default)
        => (await _api.GetAsync<IReadOnlyList<AgeStatRowDto>>("api/reports/age-stats", ct)) ?? Array.Empty<AgeStatRowDto>();

    public async Task<SummaryReportDto> GetSummaryAsync(DateTime from, DateTime to)
        => (await _api.GetAsync<SummaryReportDto>($"api/reports/summary?from={Uri.EscapeDataString(from.ToString("O"))}&to={Uri.EscapeDataString(to.ToString("O"))}"))!;
}
