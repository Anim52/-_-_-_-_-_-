using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace MaxiMed.Application.Auth
{
    public sealed class AuthService : IAuthService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;

        public AuthService(IDbContextFactory<MaxiMedDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<User?> LoginAsync(string login, string password)
        {
            await using var db = await _dbFactory.CreateDbContextAsync();

            var user = await db.Users
                .Include(u => u.Doctor)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);

            if (user is null)
                return null;

            if (!PasswordHasher.Verify(password, user.PasswordHash))
                return null;

            db.AuditLogs.Add(new AuditLog
            {
                UserId = user.Id,
                Action = "Login",
                Entity = "User",
                EntityId = user.Id.ToString(),
                DetailsJson = System.Text.Json.JsonSerializer.Serialize(new
                {
                    Login = user.Login,
                    At = DateTime.UtcNow
                })
            });

            await db.SaveChangesAsync();

            return user;
        }
    }
}
