using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class FreeSlotDto
    {
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }

        public string Day => StartAt.ToString("dd.MM.yyyy");
        public string Time => $"{StartAt:HH:mm} - {EndAt:HH:mm}";
    }
}
