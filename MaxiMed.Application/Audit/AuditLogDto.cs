using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Audit
{
    public sealed class AuditLogDto
    {
        public long Id { get; set; }
        public DateTime At { get; set; }
        public int? UserId { get; set; }
        public string? UserLogin { get; set; }
        public string Action { get; set; } = "";
        public string Entity { get; set; } = "";
        public string? EntityId { get; set; }
        public string? DetailsJson { get; set; }
    }
}
