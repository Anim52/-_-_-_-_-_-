using MaxiMed.Domain.Entities;
using MaxiMed.Domain.Lookups;
using MaxiMed.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaxiMed.Application.Invoices
{
    public sealed class PaymentService : IPaymentService
    {
        private readonly IDbContextFactory<MaxiMedDbContext> _dbFactory;
        public PaymentService(IDbContextFactory<MaxiMedDbContext> dbFactory) => _dbFactory = dbFactory;

        public async Task AddAsync(long invoiceId, PaymentMethod method, decimal amount, string? note, CancellationToken ct = default)
        {
            if (amount <= 0)
                throw new ArgumentException("Сумма должна быть больше 0");

            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var invoice = await db.Invoices.FirstOrDefaultAsync(x => x.Id == invoiceId, ct)
                ?? throw new InvalidOperationException("Счёт не найден");

            db.Payments.Add(new Payment
            {
                InvoiceId = invoiceId,
                Method = method,
                Amount = amount,
                Note = note
            });

            invoice.PaidAmount += amount;
            await db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(long paymentId, CancellationToken ct = default)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(ct);

            var p = await db.Payments.FirstOrDefaultAsync(x => x.Id == paymentId, ct);
            if (p is null) return;

            var invoice = await db.Invoices.FirstAsync(x => x.Id == p.InvoiceId, ct);
            invoice.PaidAmount -= p.Amount;

            db.Payments.Remove(p);
            await db.SaveChangesAsync(ct);
        }
    }
}
