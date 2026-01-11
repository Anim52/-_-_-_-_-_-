using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Services
{
    public sealed class ServiceDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int DurationMinutes { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsActive { get; set; }
    }
}
