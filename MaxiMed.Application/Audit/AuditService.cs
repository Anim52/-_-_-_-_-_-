using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Audit
{
    public sealed class AuditService : IAuditService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public AuditService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<AuditLogDto>> SearchAsync(AuditSearchFilter filter, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.AuditLogs.AsNoTracking()
                .Include(x => x.User)
                .AsQueryable();

            if (filter.From is not null) q = q.Where(x => x.At >= filter.From.Value);
            if (filter.To is not null) q = q.Where(x => x.At < filter.To.Value);

            if (filter.UserId is not null && filter.UserId.Value > 0)
                q = q.Where(x => x.UserId == filter.UserId.Value);

            if (!string.IsNullOrWhiteSpace(filter.Action))
                q = q.Where(x => x.Action.Contains(filter.Action));

            if (!string.IsNullOrWhiteSpace(filter.Entity))
                q = q.Where(x => x.Entity.Contains(filter.Entity));

            if (!string.IsNullOrWhiteSpace(filter.Text))
            {
                var t = filter.Text.Trim();
                q = q.Where(x =>
                    (x.EntityId ?? "").Contains(t) ||
                    (x.DetailsJson ?? "").Contains(t));
            }

            var take = filter.Take <= 0 ? 300 : Math.Min(filter.Take, 2000);

            return await q
                .OrderByDescending(x => x.At)
                .Take(take)
                .Select(x => new AuditLogDto
                {
                    Id = x.Id,
                    At = x.At,
                    UserId = x.UserId,
                    UserLogin = x.User != null ? x.User.Login : null,
                    Action = x.Action,
                    Entity = x.Entity,
                    EntityId = x.EntityId,
                    DetailsJson = x.DetailsJson
                })
                .ToListAsync(ct);
        }
    }
}
