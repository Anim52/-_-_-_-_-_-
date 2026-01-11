using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Wpf.ViewModels.Schedule
{
    public sealed class TimeSlotVm
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public bool IsFree { get; set; }

        public int? AppointmentId { get; set; }
        public string? PatientName { get; set; }
        public string? BranchName { get; set; }

        public string TimeRange => $"{StartAt:HH:mm} - {EndAt:HH:mm}";
    }
}
