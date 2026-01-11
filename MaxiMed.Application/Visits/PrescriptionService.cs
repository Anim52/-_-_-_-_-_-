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
    public sealed class PrescriptionService : IPrescriptionService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public PrescriptionService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<PrescriptionDto>> GetAsync(long visitId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Prescriptions.AsNoTracking()
                .Where(x => x.VisitId == visitId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new PrescriptionDto
                {
                    Id = x.Id,
                    VisitId = x.VisitId,
                    Text = x.Text,
                    CreatedAt = x.CreatedAt
                })
                .ToListAsync(ct);
        }

        public async Task<long> AddAsync(long visitId, string text, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Текст назначения обязателен");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = new Prescription
            {
                VisitId = visitId,
                Text = text.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            db.Prescriptions.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task DeleteAsync(long id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Prescriptions.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            db.Prescriptions.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
