using MaxiMed.Application.Patients;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

// ✅ подключи правильный namespace где лежит PatientHistoryItemDto и IPatientHistoryService
// Alt+Enter на PatientHistoryItemDto / IPatientHistoryService поможет вставить using автоматически

namespace MaxiMed.Wpf.Api
{
    public sealed class PatientHistoryApiService : IPatientHistoryService
    {
        private readonly HttpClient _http;

        public PatientHistoryApiService(HttpClient http) => _http = http;

        public async Task<IReadOnlyList<PatientHistoryItemDto>> GetAsync(
            int patientId,
            CancellationToken ct = default)
        {
            var url = $"api/patient-history/{patientId}";

            using var resp = await _http.GetAsync(url, ct);
            resp.EnsureSuccessStatusCode();

            var json = await resp.Content.ReadAsStringAsync(ct);

            var data = JsonSerializer.Deserialize<List<PatientHistoryItemDto>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            return data ?? new List<PatientHistoryItemDto>();
        }
    }
}
