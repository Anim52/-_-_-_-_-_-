using MaxiMed.Domain.Common;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Appointment : Entity<long>
    {
        public int BranchId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Planned;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int? CreatedByUserId { get; set; }
        public string? CancelReason { get; set; }

        public ClinicBranch Branch { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public Patient Patient { get; set; } = null!;
        public User? CreatedByUser { get; set; }

        public ICollection<AppointmentService> Services { get; set; } = new List<AppointmentService>();
        public Visit? Visit { get; set; }
        public Invoice? Invoice { get; set; }

    }

    public class AppointmentService : Entity<long>
    {
        public long AppointmentId { get; set; }
        public int ServiceId { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; } = 1;

        public Appointment Appointment { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }
}
