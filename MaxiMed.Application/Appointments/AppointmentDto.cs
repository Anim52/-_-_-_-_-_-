using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class AppointmentDto
    {
        public int Id { get; set; }

        public int BranchId { get; set; }
        public int DoctorId { get; set; }
        public int PatientId { get; set; }

        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public AppointmentStatus Status { get; set; }   
        public string? Notes { get; set; }

        // для грида
        public string? BranchName { get; set; }
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }
}
