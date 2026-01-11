using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public interface IInvoiceService
    {
        // загрузить/создать счёт по приёму
        Task<InvoiceDto> GetOrCreateByAppointmentAsync(long appointmentId, CancellationToken ct = default);

        // обновить скидку (на будущее)
        Task UpdateDiscountAsync(long invoiceId, decimal discountAmount, CancellationToken ct = default);

        // платежи
        Task<long> AddPaymentAsync(long invoiceId, PaymentMethod method, decimal amount, string? note, CancellationToken ct = default);
        Task DeletePaymentAsync(long paymentId, CancellationToken ct = default);
    }
}
