using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Patients
{
    public sealed class PatientHistoryItemDto
    {
        public int AppointmentId { get; set; }
        public DateTime StartAt { get; set; }
        public AppointmentStatus Status { get; set; }

        public string DoctorName { get; set; } = "";
        public string BranchName { get; set; } = "";

        public long? VisitId { get; set; }
    }

}
