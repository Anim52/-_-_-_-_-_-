using MaxiMed.Domain.Entities;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{
    public sealed class UserAdminService : IUserAdminService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public UserAdminService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;
        // roles: List<string> (например ["Doctor","Registrar"])
        private static List<string> NormalizeRoles(IEnumerable<string> roles)
            => roles.Select(r => r.Trim())
                    .Where(r => !string.IsNullOrWhiteSpace(r))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

        public async Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default)
        {
            query ??= "";
            query = query.Trim();

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var q = db.Users.AsNoTracking()
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query))
            {
                q = q.Where(u =>
                    u.Login.Contains(query) ||
                    (u.FullName ?? "").Contains(query));
            }

            var users = await q
                .OrderByDescending(u => u.CreatedAt)
                .Take(500)
                .ToListAsync(ct);

            return users.Select(u => new UserListItemDto
            {
                Id = u.Id,
                Login = u.Login,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                Roles = u.UserRoles
                    .Where(ur => ur.Role != null)
                    .Select(ur => ur.Role.Name)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList()
            }).ToList();
        }

        public async Task<UserEditDto?> GetAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var u = await db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
            if (u is null) return null;

            var roleIds = await db.UserRoles.AsNoTracking()
                .Where(ur => ur.UserId == id)
                .Select(ur => ur.RoleId)
                .ToListAsync(ct);

            return new UserEditDto
            {
                Id = u.Id,
                Login = u.Login,
                FullName = u.FullName,
                IsActive = u.IsActive,
                RoleIds = roleIds
            };
        }

        public async Task<int> CreateAsync(UserEditDto dto, string password, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Пароль обязателен при создании");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var user = new User
            {
                Login = dto.Login,
                FullName = dto.FullName,
                IsActive = dto.IsActive,
                PasswordHash = PasswordHasher.Hash(password)
            };

            db.Users.Add(user);
            await db.SaveChangesAsync(ct);

            await UpdateRolesAsync(db, user, dto.Roles, ct);
            await db.SaveChangesAsync(ct);

            return user.Id;
        }

        public async Task UpdateAsync(UserEditDto dto, string? password, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var user = await db.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == dto.Id, ct)
                ?? throw new InvalidOperationException("Пользователь не найден");

            user.Login = dto.Login;
            user.FullName = dto.FullName;
            user.IsActive = dto.IsActive;

            if (!string.IsNullOrWhiteSpace(password))
                user.PasswordHash = PasswordHasher.Hash(password);

            user.UserRoles.Clear();
            await UpdateRolesAsync(db, user, dto.Roles, ct);

            await db.SaveChangesAsync(ct);
        }
        private static async Task UpdateRolesAsync(
        MaxiMedDbContext db,
        User user,
        IEnumerable<string> roles,
        CancellationToken ct)
        {
            var roleEntities = await db.Roles
                .Where(r => roles.Contains(r.Name))
                .ToListAsync(ct);

            foreach (var role in roleEntities)
            {
                user.UserRoles.Add(new UserRole
                {
                    User = user,
                    Role = role
                });
            }
        }
        public async Task DeleteAsync(int id, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var u = await db.Users.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (u is null) return;

            // в реальности лучше не удалять, а деактивировать
            db.Users.Remove(u);
            await db.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Roles.AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => new RoleDto { Id = r.Id, Name = r.Name })
                .ToListAsync(ct);
        }

        private static async Task SetRolesAsync(MaxiMedDbContext db, int userId, List<int> roleIds, CancellationToken ct)
        {
            roleIds ??= new List<int>();

            var existing = await db.UserRoles.Where(x => x.UserId == userId).ToListAsync(ct);
            db.UserRoles.RemoveRange(existing);

            foreach (var rid in roleIds.Distinct())
                db.UserRoles.Add(new UserRole { UserId = userId, RoleId = rid });

            await db.SaveChangesAsync(ct);
        }

        private static void Validate(UserEditDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login)) throw new ArgumentException("Логин обязателен");
            if (dto.Login.Length > 64) throw new ArgumentException("Логин слишком длинный");
            if (dto.FullName?.Length > 120) throw new ArgumentException("ФИО слишком длинное");
        }
        

        private static async Task SyncRolesAsync(MaxiMedDbContext db, int userId, List<string> roles, CancellationToken ct)
        {
            roles ??= new();
            var clean = roles
                .Select(r => r.Trim())
                .Where(r => !string.IsNullOrWhiteSpace(r))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            // 1) гарантируем что роли есть в таблице Role
            var existing = await db.Roles
                .Where(r => clean.Contains(r.Name))
                .ToListAsync(ct);

            foreach (var roleName in clean)
            {
                if (!existing.Any(r => string.Equals(r.Name, roleName, StringComparison.OrdinalIgnoreCase)))
                {
                    var role = new Role { Name = roleName };
                    db.Roles.Add(role);
                    existing.Add(role);
                }
            }
            await db.SaveChangesAsync(ct); // чтобы новые роли получили Id

            // 2) удаляем старые связи
            var oldLinks = await db.UserRoles.Where(ur => ur.UserId == userId).ToListAsync(ct);
            db.UserRoles.RemoveRange(oldLinks);

            // 3) добавляем новые связи
            foreach (var role in existing)
            {
                db.UserRoles.Add(new UserRole
                {
                    UserId = userId,
                    RoleId = role.Id
                });
            }
        }
        public async Task<IReadOnlyList<string>> GetAllRoleNamesAsync(CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            return await db.Roles.AsNoTracking()
                .OrderBy(r => r.Name)
                .Select(r => r.Name)
                .ToListAsync(ct);
        }

    }
}
