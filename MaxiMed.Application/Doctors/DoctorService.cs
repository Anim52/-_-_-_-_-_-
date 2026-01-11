using MaxiMed.Application.Common;
using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Doctors
{
    public sealed class DoctorService : IDoctorService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public DoctorService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<DoctorDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            IQueryable<Doctor> q = db.Doctors.AsNoTracking()
                .Include(d => d.Branch)
                .Include(d => d.Specialty);

            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(d =>
                    d.FullName.Contains(query) ||
                    (d.Phone ?? "").Contains(query) ||
                    (d.Email ?? "").Contains(query));
            }

            return await q
                .OrderBy(d => d.FullName)
                .Select(d => new DoctorDto
                {
                    Id = d.Id,
                    FullName = d.FullName,
                    BranchId = d.BranchId,
                    SpecialtyId = d.SpecialtyId,
                    Room = d.Room,
                    Phone = d.Phone,
                    Email = d.Email,
                    IsActive = d.IsActive,
                    BranchName = d.Branch.Name,
                    SpecialtyName = d.Specialty.Name
                })
                .Take(500)
                .ToListAsync(ct);
        }


        public async Task<IReadOnlyList<LookupItemDto>> GetBranchesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.ClinicBranches.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<LookupItemDto>> GetSpecialtiesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            return await db.Specialties.AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.Name)
                .Select(x => new LookupItemDto { Id = x.Id, Name = x.Name })
                .ToListAsync(ct);
        }

        public async Task<int> CreateAsync(DoctorDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var d = new Doctor
            {
                FullName = dto.FullName.Trim(),
                BranchId = dto.BranchId,
                SpecialtyId = dto.SpecialtyId,
                Room = dto.Room,
                Phone = dto.Phone,
                Email = dto.Email,
                IsActive = dto.IsActive
            };

            db.Doctors.Add(d);
            await db.SaveChangesAsync(ct);
            return d.Id;
        }

        public async Task UpdateAsync(DoctorDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var d = await db.Doctors.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Врач не найден");

            d.FullName = dto.FullName.Trim();
            d.BranchId = dto.BranchId;
            d.SpecialtyId = dto.SpecialtyId;
            d.Room = dto.Room;
            d.Phone = dto.Phone;
            d.Email = dto.Email;
            d.IsActive = dto.IsActive;

            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);
            var d = await db.Doctors.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (d is null) return;

            // лучше архивировать
            d.IsActive = false;
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(DoctorDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName)) throw new ArgumentException("ФИО врача обязательно");
            if (dto.BranchId <= 0) throw new ArgumentException("Выбери филиал");
            if (dto.SpecialtyId <= 0) throw new ArgumentException("Выбери специализацию");
            if (dto.FullName.Length > 150) throw new ArgumentException("ФИО слишком длинное");
        }
    }
}
