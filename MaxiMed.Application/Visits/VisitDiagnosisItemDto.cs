using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public sealed class VisitDiagnosisItemDto
    {
        public int DiagnosisId { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public bool IsPrimary { get; set; }
    }
}
