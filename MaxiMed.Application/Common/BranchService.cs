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
    public sealed class BranchService : IBranchService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public BranchService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<BranchDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<ClinicBranch> q = db.ClinicBranches.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(x => x.Name.Contains(query) || (x.Address ?? "").Contains(query) || (x.Phone ?? "").Contains(query));

            return await q.OrderBy(x => x.Name)
                .Select(x => new BranchDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Address = x.Address,
                    Phone = x.Phone,
                    IsActive = x.IsActive
                })
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(BranchDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var e = new ClinicBranch
            {
                Name = dto.Name.Trim(),
                Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim(),
                Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim(),
                IsActive = dto.IsActive
            };

            db.ClinicBranches.Add(e);
            await db.SaveChangesAsync(ct);
            return e.Id;
        }

        public async Task UpdateAsync(BranchDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var e = await db.ClinicBranches.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Филиал не найден");

            e.Name = dto.Name.Trim();
            e.Address = string.IsNullOrWhiteSpace(dto.Address) ? null : dto.Address.Trim();
            e.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
            e.IsActive = dto.IsActive;

            await db.SaveChangesAsync(ct);
        }

        public async Task ArchiveAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var e = await db.ClinicBranches.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (e is null) return;
            e.IsActive = false;
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(BranchDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) throw new ArgumentException("Название филиала обязательно");
            if (dto.Name.Length > 150) throw new ArgumentException("Название слишком длинное");
            if (dto.Address?.Length > 250) throw new ArgumentException("Адрес слишком длинный");
            if (dto.Phone?.Length > 30) throw new ArgumentException("Телефон слишком длинный");
        }
    }
}
