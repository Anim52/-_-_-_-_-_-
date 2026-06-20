using MaxiMed.Application.Patients;

namespace MaxiMed.Wpf.Api
{
    public sealed class PatientHistoryApiService : IPatientHistoryService
    {
        private readonly ApiClient _api;

        public PatientHistoryApiService(ApiClient api) => _api = api;

        public async Task<IReadOnlyList<PatientHistoryItemDto>> GetAsync(
            int patientId,
            CancellationToken ct = default)
            => (await _api.GetAsync<IReadOnlyList<PatientHistoryItemDto>>($"api/patient-history/{patientId}", ct))
               ?? Array.Empty<PatientHistoryItemDto>();
    }
}
