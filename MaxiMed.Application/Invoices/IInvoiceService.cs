using MaxiMed.Domain.Lookups;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public interface IInvoiceService
    {
        // загрузить/создать счёт по приёму
        Task<InvoiceDto> GetOrCreateByAppointmentAsync(long appointmentId, CancellationToken ct = default);

        // скидка
        Task UpdateDiscountAsync(long invoiceId, decimal discountAmount, CancellationToken ct = default);

        // платежи
        Task<long> AddPaymentAsync(
            long invoiceId,
            PaymentMethod method,
            decimal amount,
            DateTime paidAt,
            string? note,
            CancellationToken ct = default);

        Task UpdatePaymentDateAsync(long paymentId, DateTime paidAt, CancellationToken ct = default);

        Task DeletePaymentAsync(long paymentId, CancellationToken ct = default);
    }
}
