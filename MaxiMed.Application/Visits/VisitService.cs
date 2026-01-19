using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public sealed class VisitService : IVisitService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public VisitService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<VisitDto> GetOrCreateByAppointmentAsync(long appointmentId, int doctorId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var v = await db.Visits.FirstOrDefaultAsync(x => x.AppointmentId == appointmentId, ct);
            if (v is null)
            {
                // Если doctorId не передали (0), берём его из записи
                if (doctorId <= 0)
                {
                    doctorId = await db.Appointments
                        .AsNoTracking()
                        .Where(a => a.Id == appointmentId)
                        .Select(a => a.DoctorId)
                        .FirstOrDefaultAsync(ct);
                }

                v = new Visit
                {
                    AppointmentId = appointmentId,
                    DoctorId = doctorId,
                    // В UI показываем "локальную" дату, поэтому используем Now
                    OpenedAt = DateTime.Now
                };
                db.Visits.Add(v);
                await db.SaveChangesAsync(ct);
            }

            return new VisitDto
            {
                Id = v.Id,
                AppointmentId = v.AppointmentId,
                DoctorId = v.DoctorId,
                OpenedAt = v.OpenedAt,
                ClosedAt = v.ClosedAt,
                Complaints = v.Complaints,
                Anamnesis = v.Anamnesis,
                Examination = v.Examination,
                DiagnosisText = v.DiagnosisText,
                Recommendations = v.Recommendations
            };
        }

        public async Task SaveAsync(VisitDto dto, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var v = await db.Visits.FirstOrDefaultAsync(x => x.AppointmentId == dto.AppointmentId, ct)
                ?? throw new InvalidOperationException("Карта приёма не найдена");

            v.DoctorId = dto.DoctorId;
            v.ClosedAt = dto.ClosedAt;
            v.Complaints = dto.Complaints;
            v.Anamnesis = dto.Anamnesis;
            v.Examination = dto.Examination;
            v.DiagnosisText = dto.DiagnosisText;
            v.Recommendations = dto.Recommendations;

            await db.SaveChangesAsync(ct);
        }
    }
}
