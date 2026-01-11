using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Common
{
    public interface ISpecialtyService
    {
        Task<IReadOnlyList<SpecialtyDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<int> CreateAsync(SpecialtyDto dto, CancellationToken ct = default);
        Task UpdateAsync(SpecialtyDto dto, CancellationToken ct = default);
        Task ArchiveAsync(int id, CancellationToken ct = default);
    }
}
