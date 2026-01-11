using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class ClinicBranch : Entity<int>
    {
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Doctor> Doctors { get; set; } = new List<Doctor>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
