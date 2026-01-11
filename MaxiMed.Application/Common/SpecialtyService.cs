using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Common
{
    public sealed class SpecialtyService : ISpecialtyService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public SpecialtyService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<SpecialtyDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<Specialty> q = db.Specialties.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(x => x.Name.Contains(query));

            return await q.OrderBy(x => x.Name)
                .Select(x => new SpecialtyDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    IsActive = x.IsActive
                })
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(SpecialtyDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var e = new Specialty
            {
                Name = dto.Name.Trim(),
                IsActive = dto.IsActive
            };

            db.Specialties.Add(e);
            await db.SaveChangesAsync(ct);
            return e.Id;
        }

        public async Task UpdateAsync(SpecialtyDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var e = await db.Specialties.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Специализация не найдена");

            e.Name = dto.Name.Trim();
            e.IsActive = dto.IsActive;

            await db.SaveChangesAsync(ct);
        }

        public async Task ArchiveAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var e = await db.Specialties.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return;
            e.IsActive = false;
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(SpecialtyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название специализации обязательно");
            if (dto.Name.Length > 120) throw new ArgumentException("Название слишком длинное");
        }
    }
}
