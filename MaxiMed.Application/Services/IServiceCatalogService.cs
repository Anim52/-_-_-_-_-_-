using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Services
{
    public interface IServiceCatalogService
    {
        Task<IReadOnlyList<ServiceDto>> SearchAsync(string? query, CancellationToken ct = default);
        Task<int> CreateAsync(ServiceDto dto, CancellationToken ct = default);
        Task UpdateAsync(ServiceDto dto, CancellationToken ct = default);
        Task DeleteAsync(int id, CancellationToken ct = default);
    }

}
