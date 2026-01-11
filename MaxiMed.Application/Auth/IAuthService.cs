using MaxiMed.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Auth
{
    public interface IAuthService
    {
        Task<User?> LoginAsync(string login, string password);
    }

}
