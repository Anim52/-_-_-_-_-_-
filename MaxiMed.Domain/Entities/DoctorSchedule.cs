using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class DoctorSchedule : Entity<int>
    {
        public int DoctorId { get; set; }
        public byte DayOfWeek { get; set; } 
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public int SlotMinutes { get; set; } = 15;
        public bool IsActive { get; set; } = true;

        public Doctor Doctor { get; set; } = null!;
    }
}
