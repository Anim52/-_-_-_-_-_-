using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public sealed class PatientDto
    {
        public int Id { get; set; }
        public string LastName { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string? MiddleName { get; set; }
        public DateTime? BirthDate { get; set; }  
        public Sex Sex { get; set; } = Sex.Unknown;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Notes { get; set; }
        public string? Snils { get; set; }
        public string? PassportNumber { get; set; }
        public string? OmsPolicyNumber { get; set; } 
        public string FullName =>
            string.Join(" ", new[] { LastName, FirstName, MiddleName }.Where(x => !string.IsNullOrWhiteSpace(x)));
    }
}
