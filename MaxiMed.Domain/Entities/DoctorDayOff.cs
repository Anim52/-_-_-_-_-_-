using MaxiMed.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class DoctorDayOff : Entity<long>
    {
        public int DoctorId { get; set; }
        public DateTime Date { get; set; } 
        public string? Reason { get; set; }

        public Doctor Doctor { get; set; } = null!;
    }
}
