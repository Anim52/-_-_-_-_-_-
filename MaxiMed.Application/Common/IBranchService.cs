using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Common
{
    public interface IBranchService
    {
        Task<IReadOnlyList<BranchDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<int> CreateAsync(BranchDto dto, CancellationToken ct = default);
        Task UpdateAsync(BranchDto dto, CancellationToken ct = default);
        Task ArchiveAsync(int id, CancellationToken ct = default);
    }
}
