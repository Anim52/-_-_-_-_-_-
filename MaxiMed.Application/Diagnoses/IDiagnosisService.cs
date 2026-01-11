using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Diagnoses
{
    public interface IDiagnosisService
    {
        Task<IReadOnlyList<DiagnosisDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<int> CreateAsync(DiagnosisDto dto, CancellationToken ct = default);
        Task UpdateAsync(DiagnosisDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
