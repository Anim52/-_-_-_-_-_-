using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Audit
{
    public interface IAuditService
    {
        Task<IReadOnlyList<AuditLogDto>> SearchAsync(AuditSearchFilter filter, CancellationToken ct = default);
    }
}
