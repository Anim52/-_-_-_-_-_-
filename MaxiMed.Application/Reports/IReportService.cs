using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Reports
{
    public interface IReportService
    {
        Task<IReadOnlyList<VisitsReportRowDto>> GetVisitsAsync(
            DateTime from, DateTime to, CancellationToken ct = default);

        Task<IReadOnlyList<RevenueReportRowDto>> GetRevenueAsync(
            DateTime from, DateTime to, CancellationToken ct = default);

        Task<IReadOnlyList<DoctorReportRowDto>> GetDoctorsAsync(
            DateTime from, DateTime to, CancellationToken ct = default);
        Task<PatientStatsReportDto> GetPatientStatsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<GenderStatRowDto>> GetGenderStatsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<AgeStatRowDto>> GetAgeStatsAsync(CancellationToken ct = default);
        Task<SummaryReportDto> GetSummaryAsync(DateTime from, DateTime to);

    }
}


