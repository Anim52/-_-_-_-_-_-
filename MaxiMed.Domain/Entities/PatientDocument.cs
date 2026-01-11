using MaxiMed.Domain.Common;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class PatientDocument : Entity<int>
    {
        public int PatientId { get; set; }
        public DocumentType DocumentType { get; set; }

        public string? Series { get; set; }
        public string Number { get; set; } = null!;
        public string? IssuedBy { get; set; }
        public DateOnly? IssuedAt { get; set; }

        public Patient Patient { get; set; } = null!;
    }

    public class InsurancePolicy : Entity<int>
    {
        public int PatientId { get; set; }
        public string Company { get; set; } = null!;
        public string PolicyNumber { get; set; } = null!;
        public DateOnly? ValidTo { get; set; }

        public Patient Patient { get; set; } = null!;
    }
}
