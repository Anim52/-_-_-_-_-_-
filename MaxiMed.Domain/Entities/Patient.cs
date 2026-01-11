using MaxiMed.Domain.Common;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Patient : Entity<int>
    {
        public string LastName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string? MiddleName { get; set; }

        public DateOnly? BirthDate { get; set; }
        public Sex Sex { get; set; } = Sex.Unknown;

        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? Snils { get; set; }
        public string? PassportNumber { get; set; }
        public string? OmsPolicyNumber { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<PatientDocument> Documents { get; set; } = new List<PatientDocument>();
        public ICollection<InsurancePolicy> InsurancePolicies { get; set; } = new List<InsurancePolicy>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
    }
}
