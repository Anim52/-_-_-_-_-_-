using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly MaxiMedDbContext _db;

        public AuthService(MaxiMedDbContext db) => _db = db;

        public async Task<User?> LoginAsync(string login, string password)
        {
            var user = await _db.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);

            if (user is null)
                return null;

            _db.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                Entity = "User",
                EntityId = user.Id.ToString()
            });
            await _db.SaveChangesAsync();

            return PasswordHasher.Verify(password, user.PasswordHash)
                ? user
                : null;
            
        }
    }
}
