using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Reports
{
    public sealed class VisitsReportRowDto
    {
        public DateTime Date { get; set; }
        public string Doctor { get; set; } = "";
        public string Patient { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Total { get; set; } 
        public decimal Paid { get; set; } 
    }

    public sealed class RevenueReportRowDto
    {
        public DateTime Date { get; set; }
        public decimal Total { get; set; }
        public decimal Paid { get; set; }
        public decimal Debt => Total - Paid;
    }

    public sealed class DoctorReportRowDto
    {
        public string Doctor { get; set; } = "";
        public int VisitsCount { get; set; }
        public decimal Revenue { get; set; }
    }
    public class SummaryReportDto
    {
        public decimal Income { get; set; }
        public decimal Expense { get; set; }
        public decimal Profit => Income - Expense;
        public decimal AvgCheck { get; set; }
        public decimal Debts { get; set; }
        public int VisitsCount { get; set; }
    }
}
