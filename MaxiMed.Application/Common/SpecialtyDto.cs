using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Common
{
    public sealed class SpecialtyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsActive { get; set; } = true;
    }
}
