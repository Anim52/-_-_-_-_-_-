using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public sealed class VisitDiagnosisService : IVisitDiagnosisService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public VisitDiagnosisService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<VisitDiagnosisItemDto>> GetAsync(long visitId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.VisitDiagnoses.AsNoTracking()
                .Where(x => x.VisitId == visitId)
                .Join(db.Diagnoses.AsNoTracking(),
                    vd => vd.DiagnosisId,
                    d => d.Id,
                    (vd, d) => new VisitDiagnosisItemDto
                    {
                        DiagnosisId = d.Id,
                        Code = d.Code,
                        Name = d.Name,
                        IsPrimary = vd.IsPrimary
                    })
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.Code)
                .ToListAsync(ct);
        }

        public async Task AddAsync(long visitId, int diagnosisId, bool isPrimary = false, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var exists = await db.VisitDiagnoses.AnyAsync(x => x.VisitId == visitId && x.DiagnosisId == diagnosisId, ct);
            if (exists) return;

            if (isPrimary)
            {
                var primaries = await db.VisitDiagnoses
                    .Where(x => x.VisitId == visitId && x.IsPrimary)
                    .ToListAsync(ct);
                foreach (var p in primaries) p.IsPrimary = false;
            }

            db.VisitDiagnoses.Add(new VisitDiagnosis
            {
                VisitId = visitId,
                DiagnosisId = diagnosisId,
                IsPrimary = isPrimary
            });

            await db.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(long visitId, int diagnosisId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.VisitDiagnoses.FirstOrDefaultAsync(x => x.VisitId == visitId && x.DiagnosisId == diagnosisId, ct);
            if (entity is null) return;

            db.VisitDiagnoses.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        public async Task SetPrimaryAsync(long visitId, int diagnosisId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var list = await db.VisitDiagnoses
                .Where(x => x.VisitId == visitId)
                .ToListAsync(ct);

            foreach (var x in list)
                x.IsPrimary = (x.DiagnosisId == diagnosisId);

            await db.SaveChangesAsync(ct);
        }
    }
}
