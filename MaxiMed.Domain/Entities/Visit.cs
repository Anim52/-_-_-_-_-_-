using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Visit : Entity<long>
    {
        public long AppointmentId { get; set; }
        public int DoctorId { get; set; }

        public DateTime OpenedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ClosedAt { get; set; }

        public string? Complaints { get; set; }
        public string? Anamnesis { get; set; }
        public string? Examination { get; set; }
        public string? DiagnosisText { get; set; }
        public string? Recommendations { get; set; }

        public Appointment Appointment { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;

        public ICollection<VisitDiagnosis> Diagnoses { get; set; } = new List<VisitDiagnosis>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }

    public class Diagnosis : Entity<int>
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public ICollection<VisitDiagnosis> Visits { get; set; } = new List<VisitDiagnosis>();
    }

    public class VisitDiagnosis
    {
        public long VisitId { get; set; }
        public int DiagnosisId { get; set; }
        public bool IsPrimary { get; set; }

        public Visit Visit { get; set; } = null!;
        public Diagnosis Diagnosis { get; set; } = null!;
    }

    public class Prescription : Entity<long>
    {
        public long VisitId { get; set; }
        public string Text { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Visit Visit { get; set; } = null!;
    }
}
