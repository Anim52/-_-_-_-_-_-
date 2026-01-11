using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public interface IPrescriptionService
    {
        Task<IReadOnlyList<PrescriptionDto>> GetAsync(long visitId, CancellationToken ct = default);
        Task<long> AddAsync(long visitId, string text, CancellationToken ct = default);
        Task DeleteAsync(long id, CancellationToken ct = default);
    }
}
