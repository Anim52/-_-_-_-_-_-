using MaxiMed.Application.Common;
using MaxiMed.Domain.Entities;
using MaxiMed.Domain.Lookups;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class AppointmentsService : IAppointmentService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public AppointmentsService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;
        public async Task<int> GetPatientIdByAppointmentAsync(long appointmentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Appointments.AsNoTracking()
                .Where(x => x.Id == appointmentId)
                .Select(x => new { x.PatientId })
                .FirstOrDefaultAsync(ct);

            if (a is null) throw new InvalidOperationException("Запись не найдена");
            return a.PatientId;
        }

        public async Task<IReadOnlyList<AppointmentDto>> GetDayAsync(DateTime day, int? doctorId, CancellationToken ct = default)
        {
            var from = day.Date;
            var to = from.AddDays(1);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<Appointment> q = db.Appointments.AsNoTracking()
                .Where(a => a.StartAt >= from && a.StartAt < to)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .Include(a => a.Branch);

            if (doctorId is not null && doctorId.Value > 0)
                q = q.Where(a => a.DoctorId == doctorId.Value);

            return await q
                .OrderBy(a => a.StartAt)
                .Select(a => new AppointmentDto
                {
                    Id = (int)a.Id,
                    BranchId = a.BranchId,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    Status = a.Status,
                    StartAt = a.StartAt,
                    EndAt = a.EndAt,

                    BranchName = a.Branch.Name,
                    DoctorName = a.Doctor.FullName,
                    PatientName = (a.Patient.LastName + " " + a.Patient.FirstName + " " + (a.Patient.MiddleName ?? "")).Trim()
                })
                .ToListAsync(ct);
        }
        public async Task CompleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new InvalidOperationException("Запись не найдена");

            a.Status = AppointmentStatus.Completed;
            await db.SaveChangesAsync(ct);
        }

        public async Task CancelAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct)
                ?? throw new InvalidOperationException("Запись не найдена");

            a.Status = AppointmentStatus.Canceled;
            await db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<LookupItemDto>> GetActiveDoctorsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Doctors.AsNoTracking()
                .Where(d => d.IsActive)
                .OrderBy(d => d.FullName)
                .Select(d => new LookupItemDto
                {
                    Id = d.Id,
                    Name = d.FullName
                })
                .ToListAsync(ct);
        }
        public async Task<LookupItemDto?> GetPatientLookupAsync(int patientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var pid = patientId;

            var p = await db.Patients.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pid, ct);

            if (p is null) return null;

            return new LookupItemDto
            {
                Id = (int)p.Id,
                Name = (p.LastName + " " + p.FirstName + " " + (p.MiddleName ?? "")).Trim()
            };
        }
        public async Task<IReadOnlyList<AppointmentDto>> GetByPatientAsync(int patientId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Appointments.AsNoTracking()
                .Where(a => a.PatientId == patientId)
                .Include(a => a.Doctor)
                .Include(a => a.Branch)
                .OrderByDescending(a => a.StartAt)
                .Select(a => new AppointmentDto
                {
                    Id = (int)a.Id,
                    BranchId = a.BranchId,
                    DoctorId = a.DoctorId,
                    PatientId = a.PatientId,
                    StartAt = a.StartAt,
                    EndAt = a.EndAt,
                    Status = a.Status,
                    DoctorName = a.Doctor.FullName,
                    BranchName = a.Branch.Name
                })
                .ToListAsync(ct);
        }
        public async Task<IReadOnlyList<FreeSlotDto>> FindFreeSlotsAsync(
    int doctorId,
    DateTime fromDate,
    DateTime toDate,
    int maxResults = 20,
    CancellationToken ct = default)
        {
            var result = new List<FreeSlotDto>();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var startDate = fromDate.Date;
            var endDate = toDate.Date.AddDays(1);

            // забираем все записи врача в диапазоне
            var appointments = await db.Appointments.AsNoTracking()
                .Where(a =>
                    a.DoctorId == doctorId &&
                    a.Status == AppointmentStatus.Planned &&
                    a.StartAt >= startDate &&
                    a.StartAt < endDate)
                .ToListAsync(ct);

            for (var day = startDate; day < endDate; day = day.AddDays(1))
            {
                var workStart = day.AddHours(9);
                var workEnd = day.AddHours(18);

                for (var t = workStart; t < workEnd; t = t.AddMinutes(30))
                {
                    var slotEnd = t.AddMinutes(30);

                    var busy = appointments.Any(a =>
                        a.StartAt < slotEnd &&
                        a.EndAt > t);

                    if (!busy)
                    {
                        result.Add(new FreeSlotDto
                        {
                            StartAt = t,
                            EndAt = slotEnd
                        });

                        if (result.Count >= maxResults)
                            return result;
                    }
                }
            }

            return result;
        }
        public async Task<IReadOnlyList<LookupItemDto>> GetActiveSpecialtiesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Specialties.AsNoTracking()
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .Select(s => new LookupItemDto
                {
                    Id = s.Id,
                    Name = s.Name
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<LookupItemDto>> GetDoctorsBySpecialtyAsync(int specialtyId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Doctors.AsNoTracking()
                .Where(d => d.IsActive && d.SpecialtyId == specialtyId)
                .OrderBy(d => d.FullName)
                .Select(d => new LookupItemDto
                {
                    Id = d.Id,
                    Name = d.FullName
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<LookupItemDto>> GetActiveBranchesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.ClinicBranches.AsNoTracking()
                .Where(b => b.IsActive)
                .OrderBy(b => b.Name)
                .Select(b => new LookupItemDto
                {
                    Id = b.Id,
                    Name = b.Name
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<LookupItemDto>> SearchPatientsAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<Patient> q = db.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(p =>
                    (p.LastName + " " + p.FirstName + " " + (p.MiddleName ?? "")).Contains(query) ||
                    (p.Phone ?? "").Contains(query) ||
                    (p.Email ?? "").Contains(query));
            }

            return await q
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Take(50)
                .Select(p => new LookupItemDto
                {
                    Id = p.Id,
                    Name = (p.LastName + " " + p.FirstName + " " + (p.MiddleName ?? "")).Trim()
                })
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(AppointmentDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            await EnsureNoDoctorOverlap(db, dto, ct);

            var a = new Appointment
            {
                BranchId = dto.BranchId,
                DoctorId = dto.DoctorId,
                PatientId = dto.PatientId,
                StartAt = dto.StartAt,
                EndAt = dto.EndAt
            };

            db.Appointments.Add(a);
            await db.SaveChangesAsync(ct);
            return (int)a.Id;
        }

        public async Task UpdateAsync(AppointmentDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            await EnsureNoDoctorOverlap(db, dto, ct);

            var a = await db.Appointments.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Запись не найдена");

            a.BranchId = dto.BranchId;
            a.DoctorId = dto.DoctorId;
            a.PatientId = dto.PatientId;
            a.StartAt = dto.StartAt;
            a.EndAt = dto.EndAt;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var a = await db.Appointments.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (a is null) return;
            var hasVisit = await db.Visits.AnyAsync(v => v.AppointmentId == id, ct);
            if (hasVisit)
                throw new InvalidOperationException("Нельзя удалить запись: по ней уже создана карта приёма (Visit).");


            db.Appointments.Remove(a);
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(AppointmentDto dto)
        {
            if (dto.BranchId <= 0) throw new ArgumentException("Выберите филиал");
            if (dto.DoctorId <= 0) throw new ArgumentException("Выберите врача");
            if (dto.PatientId <= 0) throw new ArgumentException("Выберите пациента");
            if (dto.EndAt <= dto.StartAt) throw new ArgumentException("Конец должен быть позже начала");
        }

        private static async Task EnsureNoDoctorOverlap(MaxiMedDbContext db, AppointmentDto dto, CancellationToken ct)
        {
            var exists = await db.Appointments.AsNoTracking()
                .Where(a => a.DoctorId == dto.DoctorId)
                .Where(a => a.Id != dto.Id)
                .Where(a => a.StartAt < dto.EndAt && a.EndAt > dto.StartAt)
                .AnyAsync(ct);

            if (exists)
                throw new InvalidOperationException("У врача уже есть запись на это время (пересечение).");
        }
    }
}
