using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public sealed class InvoiceDto
    {
        public long Id { get; set; }
        public long AppointmentId { get; set; }
        public int PatientId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount => TotalAmount - DiscountAmount - PaidAmount;

        public List<InvoiceItemDto> Items { get; set; } = new();
        public List<PaymentDto> Payments { get; set; } = new();
    }

    public sealed class InvoiceItemDto
    {
        public long Id { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; } = "";
        public int Qty { get; set; }
        public decimal Price { get; set; }
        public decimal LineTotal => Qty * Price;
    }

    public sealed class PaymentDto
    {
        public long Id { get; set; }
        public PaymentMethod Method { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaidAt { get; set; }
        public string? Note { get; set; }
    }
}
