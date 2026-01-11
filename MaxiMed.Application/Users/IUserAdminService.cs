using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Users
{

    public interface IUserAdminService
    {
        Task<IReadOnlyList<UserListItemDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<UserEditDto?> GetAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(UserEditDto dto, string? newPassword, CancellationToken ct = default);
        Task UpdateAsync(UserEditDto dto, string? newPassword, CancellationToken ct = default);

        Task<IReadOnlyList<RoleDto>> GetRolesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<string>> GetAllRoleNamesAsync(CancellationToken ct = default);


    }

}
