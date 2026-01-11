using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public sealed class PatientHistoryService : IPatientHistoryService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public PatientHistoryService(IDbContextFactory<MaxiMedDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<PatientHistoryItemDto>> GetAsync(int patientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Appointments.AsNoTracking()
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .Include(a => a.Visit)
                .OrderByDescending(a => a.StartAt)
                .Select(a => new PatientHistoryItemDto
                {
                    AppointmentId = (int)a.Id,
                    StartAt = a.StartAt,
                    Status = a.Status,
                    DoctorName = a.Doctor.FullName,
                    BranchName = a.Branch.Name,
                    VisitId = a.Visit != null ? a.Visit.Id : null
                })
                .ToListAsync(ct);
        }
    }
}
