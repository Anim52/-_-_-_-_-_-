using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{
    public sealed class UserEditDto
    {
        public int Id { get; set; }
        public string Login { get; set; } = "";
        public string? FullName { get; set; }
        public bool IsActive { get; set; } = true;

        // ✅ именно Id ролей (для чекбоксов/листбокса)
        public List<int> RoleIds { get; set; } = new();

        // удобно показывать в UI, если надо
        public List<string> Roles { get; set; } = new();
    }
}
