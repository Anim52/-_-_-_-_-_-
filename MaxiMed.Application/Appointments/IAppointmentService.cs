using MaxiMed.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public interface IAppointmentService
    {
        Task<IReadOnlyList<AppointmentDto>> GetDayAsync(DateTime day, int? doctorId, CancellationToken ct = default);
        Task<LookupItemDto?> GetPatientLookupAsync(int patientId, CancellationToken ct = default);
        Task<IReadOnlyList<LookupItemDto>> GetActiveDoctorsAsync(CancellationToken ct = default);
        Task<IReadOnlyList<AppointmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default);

        Task<IReadOnlyList<LookupItemDto>> GetActiveBranchesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<LookupItemDto>> SearchPatientsAsync(string? query, CancellationToken ct = default);
        Task CompleteAsync(int id, CancellationToken ct = default);
        Task CancelAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<FreeSlotDto>> FindFreeSlotsAsync(int doctorId,DateTime fromDate,DateTime toDate,int maxResults = 20,CancellationToken ct = default);
        Task<IReadOnlyList<LookupItemDto>> GetActiveSpecialtiesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<LookupItemDto>> GetDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct = default);

        Task<int> GetPatientIdByAppointmentAsync(long appointmentId, CancellationToken ct = default);

        Task<int> CreateAsync(AppointmentDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
        Task UpdateAsync(AppointmentDto dto, CancellationToken ct = default);
 
    }
}
