using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public sealed class PatientService : IPatientService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public PatientService(IDbContextFactory<MaxiMedDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<PatientDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Patients.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(p =>
                    (p.LastName + " " + p.FirstName + " " + (p.MiddleName ?? "")).Contains(query) ||
                    (p.Phone ?? "").Contains(query) ||
                    (p.Email ?? "").Contains(query));
            }

            return await q
                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
                .Select(p => new PatientDto
                {
                    Id = p.Id,
                    LastName = p.LastName,
                    FirstName = p.FirstName,
                    MiddleName = p.MiddleName,
                    BirthDate = p.BirthDate.HasValue ? p.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                    Sex = p.Sex,
                    Phone = p.Phone,
                    Email = p.Email,
                    Address = p.Address,
                    Notes = p.Notes,
                    Snils = p.Snils,
                    PassportNumber = p.PassportNumber,
                    OmsPolicyNumber = p.OmsPolicyNumber
                })
                .Take(500)
                .ToListAsync(ct);
        }

        public async Task<PatientDto?> GetAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = await db.Patients.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p is null) return null;

            return new PatientDto
            {
                Id = p.Id,
                LastName = p.LastName,
                FirstName = p.FirstName,
                MiddleName = p.MiddleName,
                BirthDate = p.BirthDate.HasValue ? p.BirthDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                Sex = p.Sex,
                Phone = p.Phone,
                Email = p.Email,
                Address = p.Address,
                Notes = p.Notes, 
                Snils = p.Snils,
                PassportNumber = p.PassportNumber,
                OmsPolicyNumber = p.OmsPolicyNumber
            };
        }

        public async Task<int> CreateAsync(PatientDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = new Patient
            {
                LastName = dto.LastName.Trim(),
                FirstName = dto.FirstName.Trim(),
                MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim(),
                BirthDate = dto.BirthDate.HasValue ? DateOnly.FromDateTime(dto.BirthDate.Value) : null,
                Sex = dto.Sex,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                Notes = dto.Notes,
                Snils = dto.Snils,
                PassportNumber = dto.PassportNumber,
                OmsPolicyNumber = dto.OmsPolicyNumber
            };

            db.Patients.Add(p);
            await db.SaveChangesAsync(ct);
            return p.Id;
        }

        public async Task UpdateAsync(PatientDto dto, CancellationToken ct = default)
        {
            Validate(dto);

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = await db.Patients.FirstOrDefaultAsync(x => x.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Пациент не найден");

            p.LastName = dto.LastName.Trim();
            p.FirstName = dto.FirstName.Trim();
            p.MiddleName = string.IsNullOrWhiteSpace(dto.MiddleName) ? null : dto.MiddleName.Trim();
            p.BirthDate = dto.BirthDate.HasValue ? DateOnly.FromDateTime(dto.BirthDate.Value) : null;
            p.Sex = dto.Sex;
            p.Phone = dto.Phone;
            p.Email = dto.Email;
            p.Address = dto.Address;
            p.Notes = dto.Notes;
            p.Snils = dto.Snils;
            p.PassportNumber = dto.PassportNumber;
            p.OmsPolicyNumber = dto.OmsPolicyNumber;


            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = await db.Patients.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (p is null) return;

            // В медсистемах часто не удаляют, а архивируют.
            // Сейчас удаляем физически (можно переделать на IsActive).
            db.Patients.Remove(p);
            await db.SaveChangesAsync(ct);
        }

        private static void Validate(PatientDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LastName)) throw new ArgumentException("Фамилия обязательна");
            if (string.IsNullOrWhiteSpace(dto.FirstName)) throw new ArgumentException("Имя обязательно");
            if (dto.LastName.Length > 80) throw new ArgumentException("Фамилия слишком длинная");
            if (dto.FirstName.Length > 80) throw new ArgumentException("Имя слишком длинное");
        }
        private static string? NormalizeSnils(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            // оставляем только цифры
            var digits = new string(value.Where(char.IsDigit).ToArray());

            // можно проверить длину (11 цифр), если хочешь
            if (digits.Length != 11)
                throw new ArgumentException("СНИЛС должен содержать 11 цифр");

            return digits;
        }
    }
}
