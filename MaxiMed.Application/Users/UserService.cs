using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{
    public sealed class UserService : IUserService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public UserService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Users.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(query))
                q = q.Where(u => u.Login.Contains(query) || (u.FullName ?? "").Contains(query));

            return await q.OrderBy(u => u.Login)
                .Select(u => new UserListItemDto
                {
                    Id = u.Id,
                    Login = u.Login,
                    FullName = u.FullName,
                    IsActive = u.IsActive
                })
                .Take(500)
                .ToListAsync(ct);
        }
    }
}
