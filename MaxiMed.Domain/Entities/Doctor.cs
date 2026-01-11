using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Doctor : Entity<int>
    {
        public string FullName { get; set; } = null!;
        public int SpecialtyId { get; set; }
        public int BranchId { get; set; }

        public string? Room { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

        public Specialty Specialty { get; set; } = null!;
        public ClinicBranch Branch { get; set; } = null!;

        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Visit> Visits { get; set; } = new List<Visit>();
    }
}
