using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public interface IPatientService
    {
        Task<IReadOnlyList<PatientDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<PatientDto?> GetAsync(int id, CancellationToken ct = default);
        Task<int> CreateAsync(PatientDto dto, CancellationToken ct = default);
        Task UpdateAsync(PatientDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }
}
