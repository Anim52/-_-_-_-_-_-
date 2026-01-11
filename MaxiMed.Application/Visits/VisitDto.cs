using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Visits
{
    public sealed class VisitDto
    {
        public long Id { get; set; }   

        public long AppointmentId { get; set; }
        public int DoctorId { get; set; }

        public DateTime OpenedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        public string? Complaints { get; set; }
        public string? Anamnesis { get; set; }
        public string? Examination { get; set; }
        public string? DiagnosisText { get; set; }
        public string? Recommendations { get; set; }
    }
}
