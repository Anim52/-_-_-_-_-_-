using MaxiMed.Domain.Common;
using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Domain.Entities
{
    public class Invoice : Entity<long>
    {
        public long AppointmentId { get; set; }
        public int PatientId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaidAmount { get; set; }

        public Appointment Appointment { get; set; } = null!;
        public Patient Patient { get; set; } = null!;

        public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class InvoiceItem : Entity<long>
    {
        public long InvoiceId { get; set; }
        public int ServiceId { get; set; }
        public int Qty { get; set; } = 1;
        public decimal Price { get; set; }

        public Invoice Invoice { get; set; } = null!;
        public Service Service { get; set; } = null!;
    }

    public class Payment : Entity<long>
    {
        public long InvoiceId { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }

        public Invoice Invoice { get; set; } = null!;
    }
}
