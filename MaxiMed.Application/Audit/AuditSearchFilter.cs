using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Audit
{
    public sealed class AuditSearchFilter
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? UserId { get; set; }
        public string? Action { get; set; }
        public string? Entity { get; set; }
        public string? Text { get; set; } 
        public int Take { get; set; } = 300;
    }
}
