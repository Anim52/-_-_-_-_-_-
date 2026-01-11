using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Doctors
{
    public sealed class DoctorDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public int BranchId { get; set; }
        public int SpecialtyId { get; set; }
        public string? Room { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

        public string? BranchName { get; set; }
        public string? SpecialtyName { get; set; }
    }
}
