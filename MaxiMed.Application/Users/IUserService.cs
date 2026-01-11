using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default);
    }
}
