using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public interface IVisitDiagnosisService
    {
        Task<IReadOnlyList<VisitDiagnosisItemDto>> GetAsync(long visitId, CancellationToken ct = default);
        Task AddAsync(long visitId, int diagnosisId, bool isPrimary = false, CancellationToken ct = default);
        Task RemoveAsync(long visitId, int diagnosisId, CancellationToken ct = default);
        Task SetPrimaryAsync(long visitId, int diagnosisId, CancellationToken ct = default);
    }
}
