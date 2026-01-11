using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Appointments
{
    public sealed class AppointmentServiceItemDto
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }

        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = "";

        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total => Qty * Price;
    }
}
