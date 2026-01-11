using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public sealed class PrescriptionDto
    {
        public long Id { get; set; }
        public long VisitId { get; set; }
        public string Text { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
