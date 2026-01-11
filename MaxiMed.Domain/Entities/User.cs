using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class User : Entity<int>
    {
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class Role : Entity<int>
    {
        public string Name { get; set; } = null!;
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }

    public class UserRole
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }

        public User User { get; set; } = null!;
        public Role Role { get; set; } = null!;
    }

    public class AuditLog : Entity<long>
    {
        public DateTime At { get; set; } = DateTime.UtcNow;
        public int? UserId { get; set; }
        public string Action { get; set; } = null!;
        public string Entity { get; set; } = null!;
        public string? EntityId { get; set; }
        public string? DetailsJson { get; set; }

        public User? User { get; set; }
    }
}
