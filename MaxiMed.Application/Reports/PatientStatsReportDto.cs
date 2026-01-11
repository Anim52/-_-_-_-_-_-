using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Reports
{
    public sealed class PatientStatsReportDto
    {
        public int TotalPatients { get; set; }
        public int RepeatPatients { get; set; }

        public List<GenderStatRowDto> Genders { get; set; } = new();
        public List<AgeStatRowDto> AgeGroups { get; set; } = new();
    }

    public sealed class GenderStatRowDto
    {
        public string GenderName { get; set; } = "";
        public int Count { get; set; }
        public double Percent { get; set; }
    }

    public sealed class AgeStatRowDto
    {
        public string AgeGroup { get; set; } = "";
        public int Count { get; set; }
        public double Percent { get; set; }
    }
}
