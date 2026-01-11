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
    public sealed class ServiceService : IServiceService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public ServiceService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

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
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var s = new Service
            {
                Name = dto.Name.Trim(),
                DurationMinutes = dto.DurationMinutes,
                BasePrice = dto.BasePrice,
                IsActive = dto.IsActive
            };

            db.Services.Add(s);
            await db.SaveChangesAsync(ct);
            return s.Id;
        }

        public async Task UpdateAsync(ServiceDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var s = await db.Services.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Услуга не найдена");

            s.Name = dto.Name.Trim();
            s.DurationMinutes = dto.DurationMinutes;
            s.BasePrice = dto.BasePrice;
            s.IsActive = dto.IsActive;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var s = await db.Services.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (s is null) return;

            db.Services.Remove(s);
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(ServiceDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название услуги обязательно");
            if (dto.Name.Length > 250) throw new ArgumentException("Название слишком длинное");
            if (dto.DurationMinutes <= 0 || dto.DurationMinutes > 600) throw new ArgumentException("Длительность некорректна");
            if (dto.BasePrice < 0) throw new ArgumentException("Цена не может быть отрицательной");
        }
    }
}
