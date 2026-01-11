
using MaxiMed.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(MaxiMedDbContext db)
        {
            // --- Роли ---
            if (!await db.Roles.AnyAsync())
            {
                db.Roles.AddRange(
                    new Role { Name = "Admin" },
                    new Role { Name = "Registrar" },
                    new Role { Name = "Doctor" }
                );
                await db.SaveChangesAsync();
            }
            var doctorRole = await db.Roles.FirstAsync(r => r.Name == "Doctor");
            var registrarRole = await db.Roles.FirstAsync(r => r.Name == "Registrar");
            // --- Администратор ---
            if (!await db.Users.AnyAsync())
            {
                var admin = new User
                {
                    Login = "admin",
                    FullName = "Администратор",
                    PasswordHash = PasswordHasher.Hash("admin123"),
                    IsActive = true
                };

                db.Users.Add(admin);
                await db.SaveChangesAsync();

                var adminRole = await db.Roles.FirstAsync(r => r.Name == "Admin");

                db.UserRoles.Add(new UserRole
                {
                    UserId = admin.Id,
                    RoleId = adminRole.Id
                });

                await db.SaveChangesAsync();
            }
            if (!await db.Users.AnyAsync(u => u.Login == "doctor"))
            {
                var doctor = new User
                {
                    Login = "doctor",
                    PasswordHash = PasswordHasher.Hash("doctor"),
                    FullName = "Врач по умолчанию",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                doctor.UserRoles.Add(new UserRole { Role = doctorRole });

                db.Users.Add(doctor);
            }

            // --- Registrar ---
            if (!await db.Users.AnyAsync(u => u.Login == "registrar"))
            {
                var reg = new User
                {
                    Login = "registrar",
                    PasswordHash = PasswordHasher.Hash("registrar"),
                    FullName = "Регистратор",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                reg.UserRoles.Add(new UserRole { Role = registrarRole });

                db.Users.Add(reg);
            }

            await db.SaveChangesAsync();
        }
    }
}

