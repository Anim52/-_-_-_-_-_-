using MaxiMed.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Doctors
{
    public interface IDoctorService
    {
        Task<IReadOnlyList<DoctorDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<int> CreateAsync(DoctorDto dto, CancellationToken ct = default);
        Task UpdateAsync(DoctorDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);

        Task<IReadOnlyList<LookupItemDto>> GetBranchesAsync(CancellationToken ct = default);
        Task<IReadOnlyList<LookupItemDto>> GetSpecialtiesAsync(CancellationToken ct = default);
    }
}
