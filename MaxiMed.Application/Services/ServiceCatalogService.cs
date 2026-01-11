using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Services
{
    public sealed class ServiceCatalogService : IServiceCatalogService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public ServiceCatalogService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<ServiceDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Services.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(s => s.Name.Contains(query));

            return await q.OrderBy(s => s.Name)
                .Select(s => new ServiceDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    DurationMinutes = s.DurationMinutes,
                    BasePrice = s.BasePrice,
                    IsActive = s.IsActive
                })
                .Take(500)
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(ServiceDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название обязательно");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = new Service
            {
                Name = dto.Name.Trim(),
                DurationMinutes = dto.DurationMinutes,
                BasePrice = dto.BasePrice,
                IsActive = dto.IsActive
            };

            db.Services.Add(entity);
            await db.SaveChangesAsync(ct);
            return entity.Id;
        }

        public async Task UpdateAsync(ServiceDto dto, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название обязательно");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Services.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Услуга не найдена");

            entity.Name = dto.Name.Trim();
            entity.DurationMinutes = dto.DurationMinutes;
            entity.BasePrice = dto.BasePrice;
            entity.IsActive = dto.IsActive;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var entity = await db.Services.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (entity is null) return;

            db.Services.Remove(entity);
            await db.SaveChangesAsync(ct);
        }
    }
}
