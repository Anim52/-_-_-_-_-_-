using MaxiMed.Domain.Lookups;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Reports
{
    public sealed class ReportService : IReportService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public ReportService(IDbContextFactory<MaxiMedDbContext> dbFactory)
            => _dbFactory = dbFactory;

        // 1️⃣ Визиты
        public async Task<IReadOnlyList<VisitsReportRowDto>> GetVisitsAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Appointments.AsNoTracking()
                .Where(a => a.StartAt >= from && a.StartAt <= to)
                .Include(a => a.Doctor)
                .Include(a => a.Patient)
                .OrderBy(a => a.StartAt)
                .Select(a => new VisitsReportRowDto
                {
                    Date = a.StartAt,
                    Doctor = a.Doctor.FullName,
                    Patient = (a.Patient.LastName + " " + a.Patient.FirstName),
                    Status = a.Status.ToString()
                })
                .ToListAsync(ct);
        }

        // 2️⃣ Выручка
        public async Task<IReadOnlyList<RevenueReportRowDto>> GetRevenueAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Invoices.AsNoTracking()
                .Where(i => i.CreatedAt >= from && i.CreatedAt <= to)
                .GroupBy(i => i.CreatedAt.Date)
                .Select(g => new RevenueReportRowDto
                {
                    Date = g.Key,
                    Total = g.Sum(x => x.TotalAmount),
                    Paid = g.Sum(x => x.PaidAmount)
                })
                .OrderBy(x => x.Date)
                .ToListAsync(ct);
        }

        // 3️⃣ По врачам
        public async Task<IReadOnlyList<DoctorReportRowDto>> GetDoctorsAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Appointments.AsNoTracking()
                .Where(a => a.StartAt >= from && a.StartAt <= to)
                .Include(a => a.Doctor)
                .GroupBy(a => a.Doctor.FullName)
                .Select(g => new DoctorReportRowDto
                {
                    Doctor = g.Key,
                    VisitsCount = g.Count(),
                    Revenue = g.Sum(a =>
                        a.Invoice != null ? a.Invoice.PaidAmount : 0)
                })
                .OrderByDescending(x => x.VisitsCount)
                .ToListAsync(ct);
        }
        public async Task<PatientStatsReportDto> GetPatientStatsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var today = DateTime.Today;

            var patients = await db.Patients
                .AsNoTracking()
                .Include(p => p.Appointments)
                .ToListAsync(ct);

            var total = patients.Count;

            // повторные пациенты — у кого 2+ записей
            var repeat = patients.Count(p => p.Appointments.Count >= 2);

            // пол
            int male = patients.Count(p => p.Sex == Sex.Male);
            int female = patients.Count(p => p.Sex == Sex.Female);
            int unknown = total - male - female;

            double Percent(int count) => total > 0
                ? Math.Round(count * 100.0 / total, 2)
                : 0.0;

            var genders = new List<GenderStatRowDto>
            {
                new() {  GenderName = "Мужской",  Count = male,    Percent = Percent(male) },
                new() { GenderName = "Женский",  Count = female,  Percent = Percent(female) },
                new() { GenderName = "Не указан",Count = unknown, Percent = Percent(unknown) }
            };

            // возраст
            int gUnder18 = 0,
                g18_24 = 0,
                g25_34 = 0,
                g35_44 = 0,
                g45Plus = 0,
                gUnknownAge = 0;

            foreach (var p in patients)
            {
                if (p.BirthDate is null)
                {
                    gUnknownAge++;
                    continue;
                }

                var birth = p.BirthDate.Value.ToDateTime(TimeOnly.MinValue);
                var age = today.Year - birth.Year;
                if (birth.Date > today.AddYears(-age)) age--; // корректировка

                if (age < 18) gUnder18++;
                else if (age <= 24) g18_24++;
                else if (age <= 34) g25_34++;
                else if (age <= 44) g35_44++;
                else g45Plus++;
            }

            var ageGroups = new List<AgeStatRowDto>
            {
                new() {  AgeGroup = "моложе 18", Count = gUnder18,  Percent = Percent(gUnder18) },
                new() { AgeGroup = "18–24",     Count = g18_24,    Percent = Percent(g18_24) },
                new() { AgeGroup = "25–34",     Count = g25_34,    Percent = Percent(g25_34) },
                new() { AgeGroup = "35–44",     Count = g35_44,    Percent = Percent(g35_44) },
                new() { AgeGroup = "45 и старше", Count = g45Plus, Percent = Percent(g45Plus) },
                new() { AgeGroup = "Не указан", Count = gUnknownAge, Percent = Percent(gUnknownAge) },
            };

            return new PatientStatsReportDto
            {
                TotalPatients = total,
                RepeatPatients = repeat,
                Genders = genders,
                AgeGroups = ageGroups
            };
        }
        public async Task<IReadOnlyList<GenderStatRowDto>> GetGenderStatsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);


            var grouped = await db.Patients
                .GroupBy(p => p.Sex)         
                .Select(g => new
                {
                    Sex = g.Key,
                    Count = g.Count()
                })
                .ToListAsync(ct);

            var total = grouped.Sum(x => x.Count);
            if (total == 0) total = 1; 

            var result = grouped
                .Select(x => new GenderStatRowDto
                {
                    GenderName = x.Sex switch
                    {
                        Sex.Male => "Мужской",
                        Sex.Female => "Женский", 
                        _ => "Не указан"
                    },
                    Count = x.Count,
                    Percent = total == 0
    ? 0
    : (double)x.Count / total * 100.0
                })
                .OrderByDescending(r => r.Count)
                .ToList();

            return result;
        }

        public async Task<SummaryReportDto> GetSummaryAsync(DateTime from, DateTime to)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            // Берём все счета за период
            var invoices = await db.Invoices
                .AsNoTracking()
                .Where(i => i.CreatedAt >= from && i.CreatedAt <= to)
                .ToListAsync();

            if (invoices.Count == 0)
            {
                return new SummaryReportDto
                {
                    Income = 0m,
                    Expense = 0m,
                    AvgCheck = 0m,
                    Debts = 0m,
                    VisitsCount = 0
                };
            }

            // Сумма по счёту = TotalAmount - DiscountAmount
            var total = invoices.Sum(i => i.TotalAmount - i.DiscountAmount);
            var paid = invoices.Sum(i => i.PaidAmount);
            var debts = total - paid;
            var count = invoices.Count;
            var avg = count > 0 ? paid / count : 0m;

            return new SummaryReportDto
            {
                Income = paid,        // фактически оплачено
                Expense = 0m,         // расходов пока нет — оставляем 0
                AvgCheck = avg,
                Debts = debts,
                VisitsCount = count
            };
        }

        public async Task<IReadOnlyList<AgeStatRowDto>> GetAgeStatsAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var births = await db.Patients
                .Where(p => p.BirthDate != null)
                .Select(p => p.BirthDate!.Value)  
                .ToListAsync(ct);                 

            if (births.Count == 0)
                return Array.Empty<AgeStatRowDto>();

            static int Age(DateOnly birth)
            {
                var today = DateOnly.FromDateTime(DateTime.Today);
                var age = today.Year - birth.Year;
                if (birth > today.AddYears(-age))
                    age--;
                return age;
            }

            var groups = new[]
            {
        new { Name = "моложе 18",   Min = 0,  Max = 17 },
        new { Name = "18–24",       Min = 18, Max = 24 },
        new { Name = "25–34",       Min = 25, Max = 34 },
        new { Name = "35–44",       Min = 35, Max = 44 },
        new { Name = "45 и старше", Min = 45, Max = 150 },
    };

            var stats = groups.Select(g =>
            {
                var cnt = births.Count(b =>
                {
                    var age = Age(b);
                    return age >= g.Min && age <= g.Max;
                });

                return new AgeStatRowDto
                {
                    AgeGroup = g.Name,
                    Count = cnt
                };
            }).ToList();

            var total = stats.Sum(x => x.Count);
            if (total > 0)
            {
                foreach (var s in stats)
                    s.Percent = Math.Round((double)s.Count * 100.0 / total, 2);
            }

            return stats;
        }
    }
}
 
