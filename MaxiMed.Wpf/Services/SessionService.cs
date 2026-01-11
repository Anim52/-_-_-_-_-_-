using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Services
{
    public sealed class SessionService : ISessionService
    {
        public User? CurrentUser { get; private set; }

        public void SetUser(User user) => CurrentUser = user;
        public void Clear() => CurrentUser = null;
        public bool IsInRole(string roleName)
        {
            if (CurrentUser is null) return false;

            return CurrentUser.UserRoles
                .Any(ur => ur.Role != null &&
                           string.Equals(ur.Role.Name, roleName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
