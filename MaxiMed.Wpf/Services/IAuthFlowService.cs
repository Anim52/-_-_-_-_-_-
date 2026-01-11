using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.Services
{
    public interface IAuthFlowService
    {
        Task<bool> LoginAndShowMainAsync();
        Task LogoutToLoginAsync();

    }
}
