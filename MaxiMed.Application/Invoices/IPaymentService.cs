using MaxiMed.Domain.Lookups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public interface IPaymentService
    {
        Task AddAsync(long invoiceId, PaymentMethod method, decimal amount, string? note, CancellationToken ct = default);
        Task DeleteAsync(long paymentId, CancellationToken ct = default);
    }
}
