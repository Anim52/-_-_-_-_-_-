using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Services
{
    public interface ISessionService
    {
        User? CurrentUser { get; }
        void SetUser(User user);
        bool IsInRole(string roleName);
        void Clear();
    }
}
