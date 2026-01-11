using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{
    public sealed class UserListItemDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public string RolesText { get; set; } = "";
        public DateTime CreatedAt { get; set; }

        public List<string> Roles { get; set; } = new();
    }
}
