using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Diagnoses
{
    public sealed class DiagnosisService : IDiagnosisService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public DiagnosisService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<DiagnosisDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Diagnoses.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(d => d.Code.Contains(query) || d.Name.Contains(query));

            return await q
                .OrderBy(d => d.Code)
                .Select(d => new DiagnosisDto { Id = d.Id, Code = d.Code, Name = d.Name })
                .Take(500)
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(DiagnosisDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = new Diagnosis
            {
                Code = dto.Code.Trim(),
                Name = dto.Name.Trim()
            };

            db.Diagnoses.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task UpdateAsync(DiagnosisDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Diagnoses.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Диагноз не найден");

            entity.Code = dto.Code.Trim();
            entity.Name = dto.Name.Trim();

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Diagnoses.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            db.Diagnoses.Remove(entity);
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(DiagnosisDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Code)) throw new ArgumentException("Код обязателен");
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название обязательно");
            if (dto.Code.Length > 32) throw new ArgumentException("Код слишком длинный");
            if (dto.Name.Length > 250) throw new ArgumentException("Название слишком длинное");
        }
    }
}
