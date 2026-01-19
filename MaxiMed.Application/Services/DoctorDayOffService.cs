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
    public sealed class DoctorDayOffService : IDoctorDayOffService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public DoctorDayOffService(IDbContextFactory<MaxiMedDbContext> dbFactory)
            => _dbFactory = dbFactory;

        public async Task AddDayOffAsync(int doctorId, DateTime date, string? reason, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var day = date.Date;

            var exists = await db.DoctorDayOffs
                .AnyAsync(x => x.DoctorId == doctorId && x.Date == day, ct);

            if (exists) return;

            db.DoctorDayOffs.Add(new DoctorDayOff
            {
                DoctorId = doctorId,
                Date = day,
                Reason = reason
            });

            await db.SaveChangesAsync(ct);
        }

        public async Task<bool> IsDayOffAsync(int doctorId, DateTime date, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var day = date.Date;

            return await db.DoctorDayOffs
                .AnyAsync(x => x.DoctorId == doctorId && x.Date == day, ct);
        }
    }
}
